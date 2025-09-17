using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace WinFormMcpServer.McpServer.Tools;

public sealed class EchoSessionIdTool : IMcpTool
{
    public string Name => "echoSessionId";

    public Tool Descriptor => new Tool
    {
        Name = Name,
        Description = "Echoes the session id back to the client.",
        InputSchema = ToolSchemas.EchoSessionId
    };

    public Task<CallToolResult> CallAsync(RequestContext<CallToolRequestParams> request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new CallToolResult
        {
            Content = [new TextContentBlock { Text = request.Server.SessionId ?? string.Empty }]
        });
    }
}