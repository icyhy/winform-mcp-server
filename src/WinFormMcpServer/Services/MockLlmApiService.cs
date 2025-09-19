using WinFormMcpServer.Models;

namespace WinFormMcpServer.Services;

/// <summary>
/// Mock LLM API服务实现
/// </summary>
public class MockLlmApiService : ILlmApiService
{
    private readonly LlmApiConfigService _configService;
    private readonly Random _random = new();

    private readonly string[] _mockResponses = new[]
    {
        "这是一个模拟的AI回复。我理解了您的问题，这里是我的回答。",
        "作为一个AI助手，我很高兴为您提供帮助。根据您的问题，我建议...",
        "感谢您的提问！基于我的理解，我认为最好的方法是...",
        "这是一个很有趣的问题。让我为您详细解释一下...",
        "我明白您的需求。在这种情况下，我建议您考虑以下几个方面...",
        "根据您提供的信息，我可以给出以下建议和分析...",
        "这个问题涉及多个方面，让我逐一为您分析...",
        "很好的问题！基于我的知识库，我可以为您提供以下信息..."
    };

    public MockLlmApiService(LlmApiConfigService configService)
    {
        _configService = configService ?? throw new ArgumentNullException(nameof(configService));
    }

    /// <summary>
    /// 发送聊天消息（模拟实现）
    /// </summary>
    /// <param name="messages">消息列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>模拟的AI回复</returns>
    public async Task<string> SendChatMessageAsync(IEnumerable<ChatMessage> messages, CancellationToken cancellationToken = default)
    {
        if (messages == null || !messages.Any())
        {
            throw new ArgumentException("消息列表不能为空", nameof(messages));
        }

        // 模拟网络延迟
        var delay = _random.Next(500, 2000);
        await Task.Delay(delay, cancellationToken);

        // 检查取消令牌
        cancellationToken.ThrowIfCancellationRequested();

        var lastMessage = messages.LastOrDefault();
        if (lastMessage == null)
        {
            return "抱歉，我没有收到您的消息。";
        }

        // 根据用户消息内容生成更相关的回复
        var userMessage = lastMessage.Content.ToLower();
        string response;

        if (userMessage.Contains("你好") || userMessage.Contains("hello"))
        {
            response = "您好！我是AI助手，很高兴为您服务。请问有什么可以帮助您的吗？";
        }
        else if (userMessage.Contains("时间") || userMessage.Contains("time"))
        {
            response = $"当前时间是：{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        }
        else if (userMessage.Contains("测试") || userMessage.Contains("test"))
        {
            response = "TOOL_CALL: {\"name\": \"InvokeTestTool\", \"arguments\": {}, \"server\": \"defaultServer\"}";
        }
        else if (userMessage.Contains("帮助") || userMessage.Contains("help"))
        {
            response = "我可以帮助您回答问题、提供建议、进行对话等。请告诉我您需要什么帮助！";
        }
        else
        {
            // 随机选择一个通用回复
            response = _mockResponses[_random.Next(_mockResponses.Length)];
        }

        // 添加一些随机性，让回复更自然
        if (_random.NextDouble() < 0.3)
        {
            response += "\n\n（这是一个模拟回复，用于测试目的）";
        }

        return response;
    }

    /// <summary>
    /// 测试API连接（模拟实现）
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>总是返回true</returns>
    public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default)
    {
        // 模拟测试延迟
        await Task.Delay(1000, cancellationToken);
        
        // Mock API总是连接成功
        return true;
    }

    /// <summary>
    /// 获取当前配置
    /// </summary>
    /// <returns>配置信息</returns>
    public LlmApiConfig GetCurrentConfig()
    {
        return _configService.GetConfig();
    }
}