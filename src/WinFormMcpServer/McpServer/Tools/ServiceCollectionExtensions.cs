using Microsoft.Extensions.DependencyInjection;

namespace WinFormMcpServer.McpServer.Tools;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register default MCP tools and supporting services. Users can replace or add their own tools via DI.
    /// </summary>
    public static IServiceCollection AddMcpTools(this IServiceCollection services)
    {
        services.AddSingleton<IMcpSamplingFactory, DefaultMcpSamplingFactory>();

        // Register local tools
        services.AddSingleton<IMcpTool, EchoTool>();
        services.AddSingleton<IMcpTool, EchoSessionIdTool>();
        services.AddSingleton<IMcpTool, SampleLlmTool>();
        services.AddSingleton<IMcpTool, InvokeTestTool>();

        return services;
    }
}