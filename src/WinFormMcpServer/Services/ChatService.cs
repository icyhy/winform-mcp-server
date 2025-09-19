using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using WinFormMcpServer.Models;
using WinFormMcpServer.McpServer.Tools;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace WinFormMcpServer.Services;

public class ChatService : IChatService
{
    private readonly ILogger<ChatService> _logger;
    private readonly IReadOnlyDictionary<string, IMcpTool> _localToolsByName;
    private readonly ILlmService _llmService;
    private readonly IMcpClientService _mcpClientService;

    public ChatService(
        ILogger<ChatService> logger, 
        IEnumerable<IMcpTool> tools,
        ILlmService llmService,
        IMcpClientService mcpClientService)
    {
        _logger = logger;
        _localToolsByName = tools.ToDictionary(t => t.Name, StringComparer.OrdinalIgnoreCase);
        _llmService = llmService;
        _mcpClientService = mcpClientService;
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
        
        // 添加本地工具（暂无）
        sb.AppendLine("本地可用的工具:");
        // foreach (var tool in _toolsByName.Values)
        // {
        //     sb.AppendLine($"- {tool.Name}: {tool.Descriptor.Description}");
        // }
        
        // 添加远程工具
        try
        {
            var remoteTools = _mcpClientService.GetAllToolsAsync().Result;
            if (remoteTools.Any())
            {
                sb.AppendLine();
                sb.AppendLine("远程可用的工具:");
				foreach (var tool in remoteTools)
                {
                    sb.AppendLine($"- {tool.Name} (来自 {tool.ServerName}): {tool.Description}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "获取远程工具列表失败");
        }
        
        sb.AppendLine();
        sb.AppendLine("当你需要调用工具时，请使用以下格式:");
        sb.AppendLine("TOOL_CALL: {\"name\": \"工具名称\", \"arguments\": {参数对象}, \"server\": \"服务器名称(可选)\"}");
        sb.AppendLine();
        sb.AppendLine("对于本地工具，server字段可以省略。对于远程工具，请指定正确的服务器名称。");
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
        if (!string.IsNullOrEmpty(toolCall.Server))
        {
            // 调用远程Mcp工具
            return await ExecuteRemoteToolCall(toolCall);
        }
        else
        {
            // 调用本地工具
            return await ExecuteLocalToolCall(toolCall);
        }
    }

    private async Task<object> ExecuteLocalToolCall(ToolCallRequest toolCall)
    {
        if (!_localToolsByName.TryGetValue(toolCall.Name, out var tool))
        {
            throw new InvalidOperationException($"未找到本地工具: {toolCall.Name}");
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
                return new[] { new TextContentBlock { Text = $"本地工具 {toolCall.Name} 执行完成" } };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行本地工具时发生错误: {ToolName}", toolCall.Name);
            return new[] { new TextContentBlock { Text = $"本地工具执行失败: {ex.Message}" } };
        }
    }

    private async Task<object> ExecuteRemoteToolCall(ToolCallRequest toolCall)
    {
        try
        {
            _logger.LogInformation("调用远程工具: {ToolName} 在服务器 {ServerName}", toolCall.Name, toolCall.Server);
            
            var result = await _mcpClientService.CallToolAsync(toolCall.Server!, toolCall.Name, toolCall.Arguments);
            
            return new[] { new TextContentBlock { Text = $"远程工具 {toolCall.Name} 执行结果: {JsonSerializer.Serialize(result)}" } };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行远程工具时发生错误: {ToolName} 在服务器 {ServerName}", toolCall.Name, toolCall.Server);
            return new[] { new TextContentBlock { Text = $"远程工具执行失败: {ex.Message}" } };
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
	    [JsonPropertyName("name")]
		public string Name { get; set; } = string.Empty;
	    [JsonPropertyName("arguments")]
		public Dictionary<string, object>? Arguments { get; set; }
	    [JsonPropertyName("server")]
		public string? Server { get; set; }
    }
}