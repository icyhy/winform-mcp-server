using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;
using System.Text;
using System.Text.Json;
using WinFormMcpServer.Models;
using ModelContextProtocol.Client;


namespace WinFormMcpServer.Services;

/// <summary>
/// MCP客户端服务实现
/// </summary>
public class McpClientService : IMcpClientService, IDisposable
{
    private readonly ILogger<McpClientService> _logger;
    private readonly Dictionary<string, McpServerInfo> _servers = new();
    private readonly Dictionary<string, IMcpClient> _serverClients = new();
    private McpClientConfig? _config;
    private readonly string _configPath;

    public event EventHandler<McpServerStatusChangedEventArgs>? ServerStatusChanged;

    public McpClientService(ILogger<McpClientService> logger)
    {
        _logger = logger;
        _configPath = Path.Combine(AppContext.BaseDirectory, "mcp-servers.json");
        
        // 初始化时加载配置并自动连接启用的服务器
        _ = Task.Run(async () =>
        {
            await LoadConfigAsync();
            await ConnectEnabledServersAsync();
        });
    }

    public async Task<List<McpServerInfo>> GetServersAsync()
    {
        await EnsureConfigLoadedAsync();
        return _servers.Values.ToList();
    }

    public async Task<bool> ConnectServerAsync(string serverName)
    {
        await EnsureConfigLoadedAsync();
        
        if (!_servers.TryGetValue(serverName, out var serverInfo))
        {
            _logger.LogWarning("服务器 {ServerName} 不存在", serverName);
            return false;
        }

        if (serverInfo.Status == McpServerStatus.Connected)
        {
            _logger.LogInformation("服务器 {ServerName} 已连接", serverName);
            return true;
        }

        try
        {
            UpdateServerStatus(serverName, McpServerStatus.Connecting);
            
            var success = await ConnectToServerAsync(serverInfo);
            
            if (success)
            {
                UpdateServerStatus(serverName, McpServerStatus.Connected);
                serverInfo.LastConnected = DateTime.UtcNow;
                
                // 获取服务器工具列表
                await RefreshServerToolsAsync(serverName);
            }
            else
            {
                UpdateServerStatus(serverName, McpServerStatus.Error, "连接失败");
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "连接服务器 {ServerName} 时发生错误", serverName);
            UpdateServerStatus(serverName, McpServerStatus.Error, ex.Message);
            return false;
        }
    }

    public async Task DisconnectServerAsync(string serverName)
    {
        if (_servers.TryGetValue(serverName, out var serverInfo))
        {
            UpdateServerStatus(serverName, McpServerStatus.Disconnected);
            
            if (_serverClients.TryGetValue(serverName, out var client))
            {
                await client.DisposeAsync();
                _serverClients.Remove(serverName);
            }
            
            serverInfo.AvailableTools.Clear();
        }
        
        await Task.CompletedTask;
    }

    public async Task ConnectAllServersAsync()
    {
        await EnsureConfigLoadedAsync();
        
        var tasks = _servers.Values
            .Where(s => s.Config.Enabled)
            .Select(s => ConnectServerAsync(s.Name))
            .ToArray();
            
        await Task.WhenAll(tasks);
    }

    public async Task DisconnectAllServersAsync()
    {
        var tasks = _servers.Keys.Select(DisconnectServerAsync).ToArray();
        await Task.WhenAll(tasks);
    }

    public async Task<List<RemoteToolInfo>> GetServerToolsAsync(string serverName)
    {
        await EnsureConfigLoadedAsync();
        
        if (_servers.TryGetValue(serverName, out var serverInfo))
        {
            return serverInfo.AvailableTools.ToList();
        }
        
        return new List<RemoteToolInfo>();
    }

    public async Task<List<RemoteToolInfo>> GetAllToolsAsync()
    {
        await EnsureConfigLoadedAsync();
        
        var allTools = new List<RemoteToolInfo>();
        foreach (var server in _servers.Values)
        {
            if (server.Status == McpServerStatus.Connected)
            {
                allTools.AddRange(server.AvailableTools);
            }
        }
        
        return allTools;
    }

    public async Task<object> CallToolAsync(string serverName, string toolName, Dictionary<string, object>? arguments = null)
    {
        await EnsureConfigLoadedAsync();
        
        if (!_servers.TryGetValue(serverName, out var serverInfo))
        {
            throw new InvalidOperationException($"服务器 {serverName} 不存在");
        }

        if (serverInfo.Status != McpServerStatus.Connected)
        {
            throw new InvalidOperationException($"服务器 {serverName} 未连接");
        }

        if (!_serverClients.TryGetValue(serverName, out var client))
        {
            throw new InvalidOperationException($"服务器 {serverName} 客户端不可用");
        }

        try
        {
            return await CallRemoteToolAsync(client, serverInfo.Config, toolName, arguments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "调用服务器 {ServerName} 工具 {ToolName} 时发生错误", serverName, toolName);
            throw;
        }
    }

    public async Task ReloadConfigAsync()
    {
        await LoadConfigAsync();
    }

    private async Task EnsureConfigLoadedAsync()
    {
        if (_config == null)
        {
            await LoadConfigAsync();
        }
    }

    private async Task LoadConfigAsync()
    {
        try
        {
            if (!File.Exists(_configPath))
            {
                _logger.LogWarning("配置文件不存在: {ConfigPath}", _configPath);
                _config = new McpClientConfig();
                return;
            }

            var json = await File.ReadAllTextAsync(_configPath);
            _config = JsonSerializer.Deserialize<McpClientConfig>(json) ?? new McpClientConfig();

            // 更新服务器信息
            _servers.Clear();
            foreach (var kvp in _config.McpServers)
            {
                _servers[kvp.Key] = new McpServerInfo
                {
                    Name = kvp.Key,
                    Config = kvp.Value,
                    Status = McpServerStatus.Disconnected
                };
            }

            _logger.LogInformation("已加载 {Count} 个MCP服务器配置", _servers.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加载配置文件失败: {ConfigPath}", _configPath);
            _config = new McpClientConfig();
        }
    }

    private async Task<bool> ConnectToServerAsync(McpServerInfo serverInfo)
    {
        var config = serverInfo.Config;
        
        switch (config.Type.ToLowerInvariant())
        {
            case McpServerType.SSE:
            case McpServerType.HTTP:
                return await ConnectHttpServerAsync(serverInfo);

            case McpServerType.Process:
            case McpServerType.Stdio:
                return await ConnectProcessServerAsync(serverInfo);
                
            default:
                _logger.LogWarning("不支持的服务器类型: {Type}", config.Type);
                return false;
        }
    }

	private async Task<bool> ConnectHttpServerAsync(McpServerInfo serverInfo)
	{
		if (string.IsNullOrEmpty(serverInfo.Config.Url))
		{
			_logger.LogWarning("HTTP服务器URL为空: {ServerName}", serverInfo.Name);
			return false;
		}

		IClientTransport clientTransport;
		try
		{
			clientTransport = new SseClientTransport(new()
			{
				Endpoint = new Uri("http://localhost:3000")
			});

			// 对于SSE类型的服务器，使用不同的连接测试方法
			if (serverInfo.Config.Type == McpServerType.SSE)
			{
				var client = await McpClientFactory.CreateAsync(clientTransport);
				await client.PingAsync();

                _serverClients[serverInfo.Name] = client;

				return true;
			}

			return false;
		}
		catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
		{
			//Task 完成，但状态是“执行超时（Faulted）
			_logger.LogError(ex, "连接超时，服务器可能未启动: {ServerName} at {Url}", serverInfo.Name, serverInfo.Config.Url);
			return false;
		}
		catch (TaskCanceledException ex)
		{
			//Task 完成，但状态是“被取消”
			_logger.LogError(ex, "请求失败，服务器可能未启动: {ServerName} at {Url}", serverInfo.Name, serverInfo.Config.Url);
			return false;
		}
		catch (Exception ex)
		{
			// Task 完成，但状态是“执行失败（Faulted）”
			_logger.LogError(ex, "连接服务器失败: {ServerName} at {Url}", serverInfo.Name, serverInfo.Config.Url);
			return false;
		}
	}


    private async Task<bool> ConnectProcessServerAsync(McpServerInfo serverInfo)
    {
        // 进程连接实现
        _logger.LogInformation("进程连接暂未实现: {ServerName}", serverInfo.Name);
        await Task.Delay(100); // 模拟连接延迟
        return false;
    }

    private async Task RefreshServerToolsAsync(string serverName)
    {
        if (!_servers.TryGetValue(serverName, out var serverInfo) || 
            !_serverClients.TryGetValue(serverName, out var client))
        {
            return;
        }

        try
        {
            var tools = await GetRemoteToolsAsync(client, serverInfo.Config);
            serverInfo.AvailableTools.Clear();
            
            foreach (var tool in tools)
            {
                tool.ServerName = serverName; // 确保设置服务器名称
                serverInfo.AvailableTools.Add(tool);
            }

            _logger.LogInformation("服务器 {ServerName} 发现 {Count} 个工具", serverName, tools.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取服务器 {ServerName} 工具列表失败", serverName);
        }
    }

    private async Task<List<RemoteToolInfo>> GetRemoteToolsAsync(IMcpClient client, McpServerConfig config)
    {
        try
        {
            // 检查对应的服务器是否已初始化
            var serverInfo = _servers.Values.FirstOrDefault(s => s.Config.Url == config.Url);
            // if (serverInfo != null && !serverInfo.IsInitialized)
            // {
            //     _logger.LogWarning("服务器 {ServerName} 尚未完成MCP协议初始化，无法获取工具列表", serverInfo.Name);
            //     return new List<RemoteToolInfo>();
            // }
            _logger.LogInformation("发送工具列表请求到: {Url}", config.Url);
            
            var tools = await client.ListToolsAsync();
            _logger.LogInformation("成功解析 {Count} 个工具", tools.Count);

            var result = new List<RemoteToolInfo>();
            foreach (var tool in tools)
            {
                result.Add(new RemoteToolInfo
                {
                    Name = tool.Name,
                    Description = tool.Description,
                    ServerName = serverInfo.Name,
                    InputSchema = tool.JsonSchema
				});
            }

            return result;

        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "请求失败，可能是服务器未正确处理tools/list请求");
            return new List<RemoteToolInfo>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取远程工具列表失败");
            return new List<RemoteToolInfo>();
        }
    }

    private async Task<object> CallRemoteToolAsync(IMcpClient client, McpServerConfig config, string toolName, Dictionary<string, object>? arguments)
    {
	    try
	    {
		    var response = await client.CallToolAsync(toolName, arguments);
		    return new
		    {
			    success = true, 
			    response = response
		    };
	    }
	    catch (TaskCanceledException ex)
	    {
		    return new { success = false };
		}
	    catch (Exception ex)
	    {
		    return new { success = false };
		}
    }

    private void UpdateServerStatus(string serverName, McpServerStatus newStatus, string? errorMessage = null)
    {
        if (_servers.TryGetValue(serverName, out var serverInfo))
        {
            var oldStatus = serverInfo.Status;
            serverInfo.Status = newStatus;
            serverInfo.ErrorMessage = errorMessage;
            
            if (newStatus == McpServerStatus.Error)
            {
                serverInfo.LastError = DateTime.UtcNow;
            }

            ServerStatusChanged?.Invoke(this, new McpServerStatusChangedEventArgs
            {
                ServerName = serverName,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                ErrorMessage = errorMessage
            });
        }
    }

    /// <summary>
    /// 自动连接所有启用的服务器
    /// </summary>
    private async Task ConnectEnabledServersAsync()
    {
        try
        {
            var enabledServers = _servers.Values.Where(s => s.Config.Enabled).ToList();
            
            if (enabledServers.Count == 0)
            {
                _logger.LogInformation("没有启用的服务器需要自动连接");
                return;
            }

            _logger.LogInformation("开始自动连接 {Count} 个启用的服务器", enabledServers.Count);

            foreach (var server in enabledServers)
            {
                try
                {
                    _logger.LogInformation("正在自动连接服务器: {ServerName}", server.Name);
                    var success = await ConnectServerAsync(server.Name);
                    
                    if (success)
                    {
                        _logger.LogInformation("服务器 {ServerName} 自动连接成功", server.Name);
                    }
                    else
                    {
                        _logger.LogWarning("服务器 {ServerName} 自动连接失败", server.Name);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "自动连接服务器 {ServerName} 时发生异常", server.Name);
                }
            }

            _logger.LogInformation("自动连接启用服务器完成");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "自动连接启用服务器时发生异常");
        }
    }

    public void ReloadConfiguration()
    {
        try
        {
            // 异步加载配置，但不等待结果
            _ = Task.Run(async () =>
            {
                try
                {
                    await LoadConfigAsync();
                    _logger.LogInformation("配置文件重新加载成功");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "重新加载配置文件失败");
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "启动配置重新加载任务失败");
        }
    }

    public void Dispose()
    {
        foreach (var client in _serverClients.Values)
        {
            client.DisposeAsync();
        }
        _serverClients.Clear();
    }
    
}