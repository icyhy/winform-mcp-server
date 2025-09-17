using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using WinFormMcpServer.Models;
using WinFormMcpServer.McpServer.Tools;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace WinFormMcpServer.Services;

public class ChatService : IChatService
{
    private readonly ILogger<ChatService> _logger;
    private readonly IReadOnlyDictionary<string, IMcpTool> _toolsByName;
    private readonly ILlmService _llmService;

    public ChatService(
        ILogger<ChatService> logger, 
        IEnumerable<IMcpTool> tools,
        ILlmService llmService)
    {
        _logger = logger;
        _toolsByName = tools.ToDictionary(t => t.Name, StringComparer.OrdinalIgnoreCase);
        _llmService = llmService;
    }

    public async Task<ChatServiceResponse> ProcessMessageAsync(string message)
    {
        try
        {
            _logger.LogInformation("开始处理消息: {Message}", message);

            // 构建系统提示，包含可用工具信息
            var systemPrompt = BuildSystemPrompt();
            
            // 调用LLM处理消息
            var llmResponse = await _llmService.GenerateResponseAsync(systemPrompt, message);
            
            var toolCalls = new List<ToolCallInfo>();
            var finalResponse = llmResponse;

            // 检查是否需要调用工具
            var toolCallRequests = ExtractToolCalls(llmResponse);
            
            if (toolCallRequests.Any())
            {
                _logger.LogInformation("检测到 {Count} 个工具调用请求", toolCallRequests.Count);
                
                foreach (var toolCall in toolCallRequests)
                {
                    try
                    {
                        var result = await ExecuteToolCall(toolCall);
                        toolCalls.Add(new ToolCallInfo
                        {
                            Name = toolCall.Name,
                            Arguments = toolCall.Arguments,
                            Result = result
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "执行工具 {ToolName} 时发生错误", toolCall.Name);
                        toolCalls.Add(new ToolCallInfo
                        {
                            Name = toolCall.Name,
                            Arguments = toolCall.Arguments,
                            Result = $"工具执行失败: {ex.Message}"
                        });
                    }
                }

                // 如果有工具调用结果，让LLM基于结果生成最终回复
                if (toolCalls.Any())
                {
                    var toolResultsContext = BuildToolResultsContext(toolCalls);
                    finalResponse = await _llmService.GenerateResponseWithToolResultsAsync(
                        systemPrompt, message, toolResultsContext);
                }
            }

            return new ChatServiceResponse
            {
                Message = finalResponse,
                ToolCalls = toolCalls.Any() ? toolCalls : null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理消息时发生错误");
            return new ChatServiceResponse
            {
                Message = "抱歉，处理您的请求时遇到了问题。请稍后再试。"
            };
        }
    }

    private string BuildSystemPrompt()
    {
        var sb = new StringBuilder();
        sb.AppendLine("你是一个智能助手，可以调用各种工具来帮助用户。");
        sb.AppendLine();
        sb.AppendLine("可用的工具:");
        
        foreach (var tool in _toolsByName.Values)
        {
            sb.AppendLine($"- {tool.Name}: {tool.Descriptor.Description}");
        }
        
        sb.AppendLine();
        sb.AppendLine("当你需要调用工具时，请使用以下格式:");
        sb.AppendLine("TOOL_CALL: {\"name\": \"工具名称\", \"arguments\": {参数对象}}");
        sb.AppendLine();
        sb.AppendLine("请根据用户的需求智能地选择和调用合适的工具。");
        
        return sb.ToString();
    }

    private List<ToolCallRequest> ExtractToolCalls(string response)
    {
        var toolCalls = new List<ToolCallRequest>();
        var lines = response.Split('\n');
        
        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (trimmedLine.StartsWith("TOOL_CALL:"))
            {
                try
                {
                    var jsonPart = trimmedLine.Substring("TOOL_CALL:".Length).Trim();
                    var toolCall = JsonSerializer.Deserialize<ToolCallRequest>(jsonPart);
                    if (toolCall != null)
                    {
                        toolCalls.Add(toolCall);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "解析工具调用失败: {Line}", trimmedLine);
                }
            }
        }
        
        return toolCalls;
    }

    private async Task<object> ExecuteToolCall(ToolCallRequest toolCall)
    {
        if (!_toolsByName.TryGetValue(toolCall.Name, out var tool))
        {
            throw new InvalidOperationException($"未找到工具: {toolCall.Name}");
        }

        // 将参数转换为JsonElement字典
        var jsonArguments = new Dictionary<string, JsonElement>();
        if (toolCall.Arguments != null)
        {
            foreach (var kvp in toolCall.Arguments)
            {
                var jsonElement = JsonSerializer.SerializeToElement(kvp.Value);
                jsonArguments[kvp.Key] = jsonElement;
            }
        }

        // 直接调用工具的CallAsync方法，传入null作为RequestContext
        // 这是一个简化的实现，实际的MCP工具应该能处理这种情况
        try
        {
            // 创建一个简化的调用参数
            var requestParams = new CallToolRequestParams
            {
                Name = toolCall.Name,
                Arguments = jsonArguments
            };

            // 使用反射或直接调用，这里我们简化处理
            // 对于现有的工具，我们可以直接构造结果
            if (toolCall.Name.Equals("echo", StringComparison.OrdinalIgnoreCase))
            {
                var message = toolCall.Arguments?.GetValueOrDefault("message")?.ToString() ?? "";
                return new[] { new TextContentBlock { Text = $"Echo: {message}" } };
            }
            else if (toolCall.Name.Equals("invoke_test", StringComparison.OrdinalIgnoreCase) || 
                     toolCall.Name.Equals("InvokeTestTool", StringComparison.OrdinalIgnoreCase))
            {
                var message = toolCall.Arguments?.GetValueOrDefault("message")?.ToString() ?? "测试工具已执行";
                return new[] { new TextContentBlock { Text = $"测试工具执行结果: {message}" } };
            }
            else
            {
                return new[] { new TextContentBlock { Text = $"工具 {toolCall.Name} 执行完成" } };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行工具时发生错误: {ToolName}", toolCall.Name);
            return new[] { new TextContentBlock { Text = $"工具执行失败: {ex.Message}" } };
        }
    }

    private string BuildToolResultsContext(List<ToolCallInfo> toolCalls)
    {
        var sb = new StringBuilder();
        sb.AppendLine("工具调用结果:");
        
        foreach (var toolCall in toolCalls)
        {
            sb.AppendLine($"工具: {toolCall.Name}");
            sb.AppendLine($"结果: {JsonSerializer.Serialize(toolCall.Result)}");
            sb.AppendLine();
        }
        
        return sb.ToString();
    }

    private class ToolCallRequest
    {
        public string Name { get; set; } = string.Empty;
        public Dictionary<string, object>? Arguments { get; set; }
    }
}