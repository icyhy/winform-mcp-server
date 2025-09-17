namespace WinFormMcpServer.McpServer;

public class McpServerHost
{
	private readonly HttpMcpServer _httpMcpServer;

	public event Action<string>? OnLogMessage;
	public bool IsRunning => _httpMcpServer.IsRunning;

	public McpServerHost()
	{
		_httpMcpServer = new HttpMcpServer();
		_httpMcpServer.OnLogMessage += message => OnLogMessage?.Invoke(message);
	}

	public async Task StartAsync(int port = 3000)
	{
		try
		{
			OnLogMessage?.Invoke($"Starting MCP Server on port {port}...");
			await _httpMcpServer.StartAsync(port);
			OnLogMessage?.Invoke("MCP Server started successfully");
		}
		catch (Exception ex)
		{
			OnLogMessage?.Invoke($"Failed to start MCP Server: {ex.Message}");
			throw;
		}
	}

	public async Task StopAsync()
	{
		try
		{
			OnLogMessage?.Invoke("Stopping MCP Server...");
			await _httpMcpServer.StopAsync();
			OnLogMessage?.Invoke("MCP Server stopped successfully");
		}
		catch (Exception ex)
		{
			OnLogMessage?.Invoke($"Error stopping MCP Server: {ex.Message}");
			throw;
		}
	}
}