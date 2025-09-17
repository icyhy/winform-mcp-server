using System.Text;
using Microsoft.Extensions.Logging;

namespace WinFormMcpServer.Services;

public class MockLlmService : ILlmService
{
    private readonly ILogger<MockLlmService> _logger;
    private readonly Random _random = new();

    public MockLlmService(ILogger<MockLlmService> logger)
    {
        _logger = logger;
    }

    public async Task<string> GenerateResponseAsync(string systemPrompt, string userMessage)
    {
        _logger.LogInformation("模拟LLM处理消息: {Message}", userMessage);
        
        // 模拟处理延迟
        await Task.Delay(_random.Next(500, 2000));

        // 简单的关键词匹配来决定是否调用工具
        var lowerMessage = userMessage.ToLower();
        
        if (lowerMessage.Contains("测试") || lowerMessage.Contains("test"))
        {
            return "我来帮您执行测试功能。\n\nTOOL_CALL: {\"name\": \"invoke_test\", \"arguments\": {\"message\": \"执行测试功能\"}}";
        }
        
        if (lowerMessage.Contains("echo") || lowerMessage.Contains("回声"))
        {
            return $"我来为您回声这条消息。\n\nTOOL_CALL: {{\"name\": \"echo\", \"arguments\": {{\"message\": \"{userMessage}\"}}}}";
        }

        // 默认回复
        var responses = new[]
        {
            "您好！我是MCP聊天助手，很高兴为您服务。",
            "我理解您的问题，让我来帮助您。",
            "这是一个很有趣的问题，我会尽力为您解答。",
            "感谢您的提问，我正在为您处理。",
            "我是基于MCP协议的智能助手，可以调用各种工具来帮助您。"
        };

        return responses[_random.Next(responses.Length)];
    }

    public async Task<string> GenerateResponseWithToolResultsAsync(string systemPrompt, string userMessage, string toolResults)
    {
        _logger.LogInformation("模拟LLM基于工具结果生成回复");
        
        // 模拟处理延迟
        await Task.Delay(_random.Next(300, 1000));

        var sb = new StringBuilder();
        sb.AppendLine("我已经成功调用了相关工具，以下是执行结果：");
        sb.AppendLine();
        
        // 简单解析工具结果
        if (toolResults.Contains("invoke_test"))
        {
            sb.AppendLine("✅ 测试工具已执行完成");
        }
        
        if (toolResults.Contains("echo"))
        {
            sb.AppendLine("✅ 回声工具已执行完成");
        }
        
        sb.AppendLine();
        sb.AppendLine("如果您需要其他帮助，请随时告诉我！");
        
        return sb.ToString();
    }
}