using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace WinFormMcpServer.McpServer.Tools;

public sealed class SampleLlmTool : IMcpTool
{
    private readonly IMcpSamplingFactory _samplingFactory;

    public SampleLlmTool(IMcpSamplingFactory samplingFactory)
    {
        _samplingFactory = samplingFactory;
    }

    public string Name => "sampleLLM";

    public Tool Descriptor => new Tool
    {
        Name = Name,
        Description = "Samples from an LLM using MCP's sampling feature.",
        InputSchema = ToolSchemas.SampleLlm
    };

    public async Task<CallToolResult> CallAsync(RequestContext<CallToolRequestParams> request, CancellationToken cancellationToken)
    {
        if (request.Params?.Arguments is null ||
            !request.Params.Arguments.TryGetValue("prompt", out var prompt) ||
            !request.Params.Arguments.TryGetValue("maxTokens", out var maxTokens))
        {
            throw new McpException("Missing required arguments 'prompt' and 'maxTokens'", McpErrorCode.InvalidParams);
        }

        var samplingParams = _samplingFactory.Create(prompt.ToString()!, Name, Convert.ToInt32(maxTokens.ToString()));
        var sampleResult = await request.Server.SampleAsync(samplingParams, cancellationToken);

        return new CallToolResult
        {
            Content = [new TextContentBlock { Text = $"LLM sampling result: {(sampleResult.Content as TextContentBlock)?.Text}" }]
        };
    }
}