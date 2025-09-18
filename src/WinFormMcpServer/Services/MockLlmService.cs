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
        await Task.Delay(_random.Next(100, 1000));

        // 简单的关键词匹配来决定是否调用工具
        var lowerMessage = userMessage.ToLower();
        
        if (lowerMessage.Contains("测试") || lowerMessage.Contains("test"))
        {
            return "我来帮您执行测试功能。\n\nTOOL_CALL: {\"name\": \"InvokeTestTool\", \"arguments\": {\"message\": \"执行测试功能\"}, \"server\": \"defaultServer\"}";
        }
        
        if (lowerMessage.Contains("echo") || lowerMessage.Contains("回声"))
        {
            return $"我来为您回声这条消息。\n\nTOOL_CALL: {{\"name\": \"echo\", \"arguments\": {{\"message\": \"{userMessage}\"}}}}";
        }

        if (lowerMessage.Contains("远程") || lowerMessage.Contains("remote"))
        {
            return "我来调用远程服务器的工具。\n\nTOOL_CALL: {\"name\": \"echo\", \"arguments\": {\"message\": \"远程工具测试\"}, \"server\": \"defaultServer\"}";
        }

        if (lowerMessage.Contains("工具列表") || lowerMessage.Contains("可用工具"))
        {
            return "让我为您查看当前可用的工具列表。根据系统提示，我可以看到本地和远程的工具。您可以尝试说'测试'来调用本地工具，或者说'远程测试'来调用远程工具。";
        }

        // 默认回复
        var responses = new[]
        {
            "您好！我是MCP聊天助手，很高兴为您服务。我可以调用本地和远程的各种工具来帮助您。",
            "我理解您的问题，让我来帮助您。您可以尝试说'测试'、'echo'或'远程测试'来体验工具调用功能。",
            "这是一个很有趣的问题，我会尽力为您解答。我可以访问多个MCP服务器上的工具。",
            "感谢您的提问，我正在为您处理。试试说'工具列表'来了解我能做什么。",
            "我是基于MCP协议的智能助手，可以调用本地和远程服务器上的各种工具来帮助您。"
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