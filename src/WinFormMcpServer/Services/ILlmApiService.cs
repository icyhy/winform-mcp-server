using WinFormMcpServer.Models;

namespace WinFormMcpServer.Services;

/// <summary>
/// LLM API服务接口
/// </summary>
public interface ILlmApiService
{
    /// <summary>
    /// 发送聊天消息
    /// </summary>
    /// <param name="messages">消息列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>AI回复</returns>
    Task<string> SendChatMessageAsync(IEnumerable<ChatMessage> messages, CancellationToken cancellationToken = default);

    /// <summary>
    /// 测试API连接
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>测试结果</returns>
    Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取当前配置
    /// </summary>
    /// <returns>配置信息</returns>
    LlmApiConfig GetCurrentConfig();
}

/// <summary>
/// 聊天消息模型
/// </summary>
public class ChatMessage
{
    /// <summary>
    /// 角色（user, assistant, system）
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// 消息内容
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 时间戳
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;
}