using WinFormMcpServer.Models;

namespace WinFormMcpServer.Services;

public interface IChatService
{
    Task<ChatServiceResponse> ProcessMessageAsync(string message);
}