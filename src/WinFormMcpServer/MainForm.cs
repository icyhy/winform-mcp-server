using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using System.Text.Json;
using WinFormMcpServer.McpServer;
using WinFormMcpServer.McpServer.Tools;
using WinFormMcpServer.Services;
using Microsoft.Extensions.Logging;

namespace WinFormMcpServer;

public partial class MainForm : Form
{
	private McpServerHost? _mcpServer;
	private bool _isServerRunning = false;
	private IMcpClientService? _mcpClientService;
    private LlmApiConfigService? _llmApiConfigService;
    private LlmApiServiceFactory? _llmApiServiceFactory;

	public MainForm()
	{
		InitializeComponent();
		_mcpServer = new McpServerHost(this);
		
		// 初始化MCP客户端服务
		InitializeMcpClientService();
		
		// 初始化LLM API配置服务
		InitializeLlmApiConfigService();
	}

	private void InitializeMcpClientService()
	{
		try
		{
			var logger = new ConsoleLogger<McpClientService>();
			_mcpClientService = new McpClientService(logger);
			
			// 订阅服务器状态变化事件
			_mcpClientService.ServerStatusChanged += OnMcpServerStatusChanged;
		}
		catch (Exception ex)
		{
			LogMessage($"初始化MCP客户端服务失败: {ex.Message}");
		}
	}

	private void OnMcpServerStatusChanged(object? sender, McpServerStatusChangedEventArgs e)
	{
		LogMessage($"MCP服务器 {e.ServerName} 状态变化: {e.OldStatus} -> {e.NewStatus}");
		if (!string.IsNullOrEmpty(e.ErrorMessage))
		{
			LogMessage($"错误信息: {e.ErrorMessage}");
		}
	}

	private void InitializeLlmApiConfigService()
	{
		try
		{
			_llmApiConfigService = new LlmApiConfigService();
			_llmApiServiceFactory = new LlmApiServiceFactory(_llmApiConfigService);
			LogMessage("LLM API配置服务初始化成功");
		}
		catch (Exception ex)
		{
			LogMessage($"初始化LLM API配置服务失败: {ex.Message}");
		}
	}



	private void LogMessage(string message)
	{
		if (txtLog.InvokeRequired)
		{
			txtLog.Invoke(new Action<string>(LogMessage), message);
		}
		else
		{
			txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
			txtLog.ScrollToCaret();
		}
	}

	private async void btnStartStop_Click(object sender, EventArgs e)
	{
		if (!_isServerRunning)
		{
			// 启动服务器
			try
			{
				var portText = txtPort.Text.Trim();
				if (!int.TryParse(portText, out int port) || port <= 0 || port > 65535)
				{
					MessageBox.Show("请输入有效的端口号 (1-65535)", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				btnStartStop.Enabled = false;
				btnStartStop.Text = "启动中...";

				_mcpServer = new McpServerHost(this);
				_mcpServer.OnLogMessage += LogMessage;

				await _mcpServer.StartAsync(port);

				_isServerRunning = true;
				btnStartStop.Text = "停止服务器";
				btnStartStop.Enabled = true;
				txtPort.Enabled = false;
				lblStatus.Text = "运行中";
				lblStatus.ForeColor = Color.Green;

				// 服务器启动后，尝试连接所有启用的MCP客户端服务器
				if (_mcpClientService != null)
				{
					LogMessage("正在连接MCP客户端服务器...");
					await _mcpClientService.ConnectAllServersAsync();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"启动服务器失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
				btnStartStop.Text = "启动服务器";
				btnStartStop.Enabled = true;
				txtPort.Enabled = true;
				_isServerRunning = false;
				lblStatus.Text = "已停止";
				lblStatus.ForeColor = Color.Red;
			}
		}
		else
		{
			// 停止服务器
			await StopServerAsync();
		}
	}

	private async Task StopServerAsync()
	{
		if (_mcpServer == null || !_isServerRunning)
			return;

		try
		{
			btnStartStop.Enabled = false;
			btnStartStop.Text = "停止中...";

			// 使用CancellationToken和超时控制
			using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
			
			try
			{
				await _mcpServer.StopAsync(cts.Token);
			}
			catch (OperationCanceledException)
			{
				LogMessage("服务器停止超时，正在强制终止...");
				
				// 强制终止：直接设置状态
				_isServerRunning = false;
				LogMessage("服务器已强制终止");
			}

			_isServerRunning = false;
			btnStartStop.Text = "启动服务器";
			btnStartStop.Enabled = true;
			txtPort.Enabled = true;
			lblStatus.Text = "已停止";
			lblStatus.ForeColor = Color.Red;

			// 清理服务器实例
			if (_mcpServer != null)
			{
				_mcpServer.OnLogMessage -= LogMessage;
				_mcpServer = null;
			}
		}
		catch (Exception ex)
		{
			LogMessage($"停止服务器时发生错误: {ex.Message}");
			
			// 即使出现异常，也要重置UI状态
			_isServerRunning = false;
			btnStartStop.Text = "启动服务器";
			btnStartStop.Enabled = true;
			txtPort.Enabled = true;

			// 清理服务器实例
			if (_mcpServer != null)
			{
				_mcpServer.OnLogMessage -= LogMessage;
				_mcpServer = null;
			}
		}
	}

	private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (_isServerRunning)
		{
			// 在窗体关闭前停止服务器
			try
			{
				// 使用更短的超时时间，避免阻塞窗体关闭
				using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
				await _mcpServer?.StopAsync(cts.Token);
			}
			catch (OperationCanceledException)
			{
				// 超时时强制终止
				LogMessage("窗体关闭时服务器停止超时，已强制终止");
			}
			catch (Exception ex)
			{
				// 记录错误但不阻止窗体关闭
				LogMessage($"窗体关闭时停止服务器出错: {ex.Message}");
			}
		}
		
		// 清理MCP客户端服务资源
		if (_mcpClientService != null)
		{
			try
			{
				_mcpClientService.ServerStatusChanged -= OnMcpServerStatusChanged;
				
				// 使用超时控制断开连接
				using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
				var disconnectTask = _mcpClientService.DisconnectAllServersAsync();
				
				try
				{
					await disconnectTask.WaitAsync(cts.Token);
				}
				catch (TimeoutException)
				{
					// 超时时继续清理
				}
				
				if (_mcpClientService is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
			catch (Exception ex)
			{
				// 记录错误但不阻止窗体关闭
				LogMessage($"清理MCP客户端服务时出错: {ex.Message}");
			}
		}
	}

	delegate void TestButtonClickDelegate();
	public void InvokeTestButtonClick()
	{
		Task.Run(()=>
		{
			if (this.InvokeRequired)
			{
				TestButtonClickDelegate d = new TestButtonClickDelegate(InvokeTestButton);
				this.Invoke(d);
			}
			else
			{
				InvokeTestButton();
			}
		});
		
	}

	private void InvokeTestButton()
	{
		TestButton_Click(this, EventArgs.Empty);
	}

	private void TestButton_Click(object sender, EventArgs e)
	{
		MessageBox.Show("This is a test!");
	}

	private void mcpConfigToolStripMenuItem_Click(object sender, EventArgs e)
	{
		// 打开MCP配置窗口
		var configForm = new McpConfigForm(_mcpClientService);
		configForm.ShowDialog();
	}

	private void llmApiConfigToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (_llmApiConfigService == null)
		{
			MessageBox.Show("LLM API配置服务未初始化", "错误", 
				MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}

		// 打开LLM API配置窗口
		var configForm = new LlmApiConfigForm(_llmApiConfigService);
		configForm.ShowDialog();
	}
}