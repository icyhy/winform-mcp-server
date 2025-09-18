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

	public MainForm()
	{
		InitializeComponent();
		_mcpServer = new McpServerHost(this);
		
		// 初始化MCP客户端服务
		InitializeMcpClientService();
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

	private void UpdateServerStatus()
	{
		// 更新UI状态
		btnStartStop.Text = _isServerRunning ? "停止服务器" : "启动服务器";
		txtPort.Enabled = !_isServerRunning;
		lblStatus.Text = _isServerRunning ? "运行中" : "已停止";
		lblStatus.ForeColor = _isServerRunning ? Color.Green : Color.Red;
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
		if (_isServerRunning)
		{
			// 停止服务器
			try
			{
				await _mcpServer!.StopAsync();
				_isServerRunning = false;
				LogMessage("MCP服务器已停止");
			}
			catch (Exception ex)
			{
				LogMessage($"停止服务器时出错: {ex.Message}");
			}
		}
		else
		{
			// 启动服务器
			try
			{
				if (!int.TryParse(txtPort.Text, out int port))
				{
					MessageBox.Show("请输入有效的端口号", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				_mcpServer = new McpServerHost(this);
				_mcpServer.OnLogMessage += LogMessage;

				await _mcpServer.StartAsync(port);
				_isServerRunning = true;
				LogMessage($"MCP服务器已在端口 {port} 上启动");
				
				// 服务器启动后，尝试连接所有启用的MCP客户端服务器
				if (_mcpClientService != null)
				{
					LogMessage("正在连接MCP客户端服务器...");
					await _mcpClientService.ConnectAllServersAsync();
				}
			}
			catch (Exception ex)
			{
				LogMessage($"启动服务器时出错: {ex.Message}");
			}
		}

		UpdateServerStatus();
	}

	private void Form1_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (_isServerRunning)
		{
			// 在窗体关闭前停止服务器
			try
			{
				_mcpServer?.StopAsync().Wait(1000); // 等待最多1秒
			}
			catch
			{
				// 忽略停止服务器时的任何错误
			}
		}
		
		// 清理MCP客户端服务资源
		if (_mcpClientService != null)
		{
			try
			{
				_mcpClientService.ServerStatusChanged -= OnMcpServerStatusChanged;
				_mcpClientService.DisconnectAllServersAsync().Wait(1000);
				if (_mcpClientService is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
			catch
			{
				// 忽略清理时的任何错误
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
}