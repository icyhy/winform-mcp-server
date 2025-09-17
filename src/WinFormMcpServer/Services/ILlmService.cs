namespace WinFormMcpServer.Services;

public interface ILlmService
{
    Task<string> GenerateResponseAsync(string systemPrompt, string userMessage);
    Task<string> GenerateResponseWithToolResultsAsync(string systemPrompt, string userMessage, string toolResults);
}