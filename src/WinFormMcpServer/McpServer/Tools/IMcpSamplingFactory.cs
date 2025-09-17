using ModelContextProtocol.Protocol;

namespace WinFormMcpServer.McpServer.Tools;

public interface IMcpSamplingFactory
{
    CreateMessageRequestParams Create(string context, string uri, int maxTokens = 100);
}

internal sealed class DefaultMcpSamplingFactory : IMcpSamplingFactory
{
    public CreateMessageRequestParams Create(string context, string uri, int maxTokens = 100)
    {
        return new CreateMessageRequestParams
        {
            Messages = [new SamplingMessage
            {
                Role = Role.User,
                Content = new TextContentBlock { Text = $"Resource {uri} context: {context}" },
            }],
            SystemPrompt = "You are a helpful test server.",
            MaxTokens = maxTokens,
            Temperature = 0.7f,
            IncludeContext = ContextInclusion.ThisServer
        };
    }
}