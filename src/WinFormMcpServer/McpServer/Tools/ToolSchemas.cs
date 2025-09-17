using System.Text.Json;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;

namespace WinFormMcpServer.McpServer.Tools;

internal static class ToolSchemas
{
    public static readonly JsonElement Echo = JsonSerializer.Deserialize<JsonElement>(
        """
        {
            "type": "object",
            "properties": {
                "message": {
                    "type": "string",
                    "description": "The input to echo back."
                }
            },
            "required": ["message"]
        }
        """, McpJsonUtilities.DefaultOptions);

    public static readonly JsonElement EchoSessionId = JsonSerializer.Deserialize<JsonElement>(
        """
        {
            "type": "object"
        }
        """, McpJsonUtilities.DefaultOptions);

    public static readonly JsonElement SampleLlm = JsonSerializer.Deserialize<JsonElement>(
        """
        {
            "type": "object",
            "properties": {
                "prompt": {
                    "type": "string",
                    "description": "The prompt to send to the LLM"
                },
                "maxTokens": {
                    "type": "number",
                    "description": "Maximum number of tokens to generate"
                }
            },
            "required": ["prompt", "maxTokens"]
        }
        """, McpJsonUtilities.DefaultOptions);
}