using System.Text.Json.Serialization;

namespace WinFormMcpServer.Models;

/// <summary>
/// MCP客户端配置根对象
/// </summary>
public class McpClientConfig
{
    [JsonPropertyName("mcpServers")]
    public Dictionary<string, McpServerConfig> McpServers { get; set; } = new();
}

/// <summary>
/// 单个MCP服务器配置
/// </summary>
public class McpServerConfig
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("command")]
    public string? Command { get; set; }

    [JsonPropertyName("args")]
    public string[]? Args { get; set; }

    [JsonPropertyName("env")]
    public Dictionary<string, string>? Env { get; set; }

    [JsonPropertyName("cwd")]
    public string? Cwd { get; set; }

    [JsonPropertyName("note")]
    public string? Note { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; } = true;

    [JsonPropertyName("timeout")]
    public int Timeout { get; set; } = 30000; // 30秒超时

    [JsonPropertyName("retryCount")]
    public int RetryCount { get; set; } = 3;

    [JsonPropertyName("headers")]
    public Dictionary<string, string>? Headers { get; set; }
}

/// <summary>
/// MCP服务器连接类型
/// </summary>
public static class McpServerType
{
    public const string SSE = "sse";
    public const string WebSocket = "websocket";
    public const string HTTP = "http";
    public const string Process = "process";
    public const string Stdio = "stdio";
}

/// <summary>
/// MCP服务器连接状态
/// </summary>
public enum McpServerStatus
{
    Disconnected,
    Connecting,
    Connected,
    Error,
    Timeout
}

/// <summary>
/// MCP服务器运行时信息
/// </summary>
public class McpServerInfo
{
    public string Name { get; set; } = string.Empty;
    public McpServerConfig Config { get; set; } = new();
    public McpServerStatus Status { get; set; } = McpServerStatus.Disconnected;
    public string? ErrorMessage { get; set; }
    public DateTime? LastConnected { get; set; }
    public DateTime? LastError { get; set; }
    public List<RemoteToolInfo> AvailableTools { get; set; } = new();
    
    /// <summary>
    /// MCP协议是否已初始化
    /// </summary>
    public bool IsInitialized { get; set; } = false;
    
    /// <summary>
    /// 服务器信息（从initialize响应获取）
    /// </summary>
    public McpServerCapabilities? ServerCapabilities { get; set; }
}

/// <summary>
/// MCP服务器能力信息
/// </summary>
public class McpServerCapabilities
{
    public string? Name { get; set; }
    public string? Version { get; set; }
    public McpCapabilities? Capabilities { get; set; }
}

/// <summary>
/// MCP协议能力定义
/// </summary>
public class McpCapabilities
{
    public bool Tools { get; set; }
    public bool Resources { get; set; }
    public bool Prompts { get; set; }
    public bool Logging { get; set; }
}

/// <summary>
/// 远程工具信息
/// </summary>
public class RemoteToolInfo
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ServerName { get; set; } = string.Empty;
    public object? InputSchema { get; set; }
}