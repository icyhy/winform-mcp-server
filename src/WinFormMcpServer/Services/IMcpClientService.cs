using WinFormMcpServer.Models;

namespace WinFormMcpServer.Services;

/// <summary>
/// MCP客户端服务接口
/// </summary>
public interface IMcpClientService
{
    /// <summary>
    /// 获取所有配置的MCP服务器信息
    /// </summary>
    Task<List<McpServerInfo>> GetServersAsync();

    /// <summary>
    /// 连接到指定的MCP服务器
    /// </summary>
    Task<bool> ConnectServerAsync(string serverName);

    /// <summary>
    /// 断开指定的MCP服务器连接
    /// </summary>
    Task DisconnectServerAsync(string serverName);

    /// <summary>
    /// 连接到所有启用的MCP服务器
    /// </summary>
    Task ConnectAllServersAsync();

    /// <summary>
    /// 断开所有MCP服务器连接
    /// </summary>
    Task DisconnectAllServersAsync();

    /// <summary>
    /// 获取指定服务器的可用工具列表
    /// </summary>
    Task<List<RemoteToolInfo>> GetServerToolsAsync(string serverName);

    /// <summary>
    /// 获取所有服务器的可用工具列表
    /// </summary>
    Task<List<RemoteToolInfo>> GetAllToolsAsync();

    /// <summary>
    /// 调用指定服务器的工具
    /// </summary>
    Task<object> CallToolAsync(string serverName, string toolName, Dictionary<string, object>? arguments = null);

    /// <summary>
    /// 重新加载配置文件
    /// </summary>
    Task ReloadConfigAsync();

    /// <summary>
    /// 重新加载配置文件（同步版本）
    /// </summary>
    void ReloadConfiguration();

    /// <summary>
    /// 服务器状态变化事件
    /// </summary>
    event EventHandler<McpServerStatusChangedEventArgs>? ServerStatusChanged;
}

/// <summary>
/// MCP服务器状态变化事件参数
/// </summary>
public class McpServerStatusChangedEventArgs : EventArgs
{
    public string ServerName { get; set; } = string.Empty;
    public McpServerStatus OldStatus { get; set; }
    public McpServerStatus NewStatus { get; set; }
    public string? ErrorMessage { get; set; }
}