using System.Text.Json.Serialization;

namespace WinFormMcpServer.Models;

public class ChatRequest
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp { get; set; }
}

public class ChatResponse
{
    [JsonPropertyName("response")]
    public string Response { get; set; } = string.Empty;

    [JsonPropertyName("toolCalls")]
    public List<ToolCallInfo>? ToolCalls { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
}

public class ToolCallInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("arguments")]
    public Dictionary<string, object>? Arguments { get; set; }

    [JsonPropertyName("result")]
    public object? Result { get; set; }
}

public class ChatServiceResponse
{
    public string Message { get; set; } = string.Empty;
    public List<ToolCallInfo>? ToolCalls { get; set; }
}