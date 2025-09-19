using Microsoft.Extensions.Logging;
using WinFormMcpServer.Models;

namespace WinFormMcpServer.Services;

/// <summary>
/// 可配置的LLM服务实现，根据配置选择Mock或真实API
/// </summary>
public class ConfigurableLlmService : ILlmService
{
    private readonly ILogger<ConfigurableLlmService> _logger;
    private readonly LlmApiServiceFactory _serviceFactory;

    public ConfigurableLlmService(ILogger<ConfigurableLlmService> logger, LlmApiServiceFactory serviceFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
    }

    /// <summary>
    /// 生成回复
    /// </summary>
    /// <param name="systemPrompt">系统提示</param>
    /// <param name="userMessage">用户消息</param>
    /// <returns>AI回复</returns>
    public async Task<string> GenerateResponseAsync(string systemPrompt, string userMessage)
    {
        try
        {
            var service = _serviceFactory.GetService();
            
            // 构建消息列表
            var messages = new List<ChatMessage>();
            
            // 添加系统消息（如果有）
            if (!string.IsNullOrWhiteSpace(systemPrompt))
            {
                messages.Add(new ChatMessage
                {
                    Role = "system",
                    Content = systemPrompt,
                    Timestamp = DateTime.UtcNow
                });
            }
            
            // 添加用户消息
            messages.Add(new ChatMessage
            {
                Role = "user",
                Content = userMessage,
                Timestamp = DateTime.UtcNow
            });

            _logger.LogInformation("调用LLM API生成回复，消息数量: {Count}", messages.Count);
            
            var response = await service.SendChatMessageAsync(messages);
            
            _logger.LogInformation("LLM API回复成功，回复长度: {Length}", response?.Length ?? 0);
            
            return response ?? "抱歉，我无法生成回复。";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成回复时发生错误");
            return "抱歉，AI服务暂时不可用，请稍后再试。";
        }
    }

    /// <summary>
    /// 基于工具结果生成回复
    /// </summary>
    /// <param name="systemPrompt">系统提示</param>
    /// <param name="userMessage">用户消息</param>
    /// <param name="toolResults">工具执行结果</param>
    /// <returns>AI回复</returns>
    public async Task<string> GenerateResponseWithToolResultsAsync(string systemPrompt, string userMessage, string toolResults)
    {
        try
        {
            var service = _serviceFactory.GetService();
            
            // 构建消息列表
            var messages = new List<ChatMessage>();
            
            // 添加系统消息（如果有）
            if (!string.IsNullOrWhiteSpace(systemPrompt))
            {
                messages.Add(new ChatMessage
                {
                    Role = "system",
                    Content = systemPrompt,
                    Timestamp = DateTime.UtcNow
                });
            }
            
            // 添加用户消息
            messages.Add(new ChatMessage
            {
                Role = "user",
                Content = userMessage,
                Timestamp = DateTime.UtcNow
            });
            
            // 添加工具结果作为助手消息
            messages.Add(new ChatMessage
            {
                Role = "assistant",
                Content = $"我已经执行了相关工具，结果如下：\n{toolResults}\n\n基于这些结果，我来为您总结和回答：",
                Timestamp = DateTime.UtcNow
            });
            
            // 添加一个新的用户消息，要求基于工具结果回答
            messages.Add(new ChatMessage
            {
                Role = "user",
                Content = "请基于上述工具执行结果，为我提供完整的回答。",
                Timestamp = DateTime.UtcNow
            });

            _logger.LogInformation("调用LLM API生成基于工具结果的回复，消息数量: {Count}", messages.Count);
            
            var response = await service.SendChatMessageAsync(messages);
            
            _logger.LogInformation("LLM API回复成功，回复长度: {Length}", response?.Length ?? 0);
            
            return response ?? "抱歉，我无法基于工具结果生成回复。";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "基于工具结果生成回复时发生错误");
            return "抱歉，AI服务暂时不可用，请稍后再试。";
        }
    }
}