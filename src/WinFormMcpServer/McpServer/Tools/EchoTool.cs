using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace WinFormMcpServer.McpServer.Tools;

public sealed class EchoTool : IMcpTool
{
    public string Name => "echo";

    public Tool Descriptor => new Tool
    {
        Name = Name,
        Description = "Echoes the input back to the client.",
        InputSchema = ToolSchemas.Echo
    };

    public Task<CallToolResult> CallAsync(RequestContext<CallToolRequestParams> request, CancellationToken cancellationToken)
    {
        if (request.Params?.Arguments is null || !request.Params.Arguments.TryGetValue("message", out var message))
        {
            throw new McpException("Missing required argument 'message'", McpErrorCode.InvalidParams);
        }

        return Task.FromResult(new CallToolResult
        {
            Content = [new TextContentBlock { Text = $"Echo: {message}" }]
        });
    }
}