using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace WinFormMcpServer.McpServer.Tools;

/// <summary>
/// Represents a single MCP tool with its descriptor and execution logic.
/// </summary>
public interface IMcpTool
{
    /// <summary>Unique tool name. Must match <see cref="Tool.Name"/>.</summary>
    string Name { get; }

    /// <summary>Descriptor exposed via ListTools.</summary>
    Tool Descriptor { get; }

    /// <summary>
    /// Executes the tool logic for a given request.
    /// </summary>
    /// <param name="request">CallTool request wrapper.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Tool result.</returns>
    Task<CallToolResult> CallAsync(RequestContext<CallToolRequestParams> request, CancellationToken cancellationToken);
}