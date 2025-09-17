using WinFormMcpServer.McpServer;

namespace WinFormMcpServer;

public partial class MainForm : Form
{
    private McpServerHost? _mcpServer;
    private bool _isServerRunning = false;

    public MainForm()
    {
        InitializeComponent();
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

                _mcpServer = new McpServerHost();
                _mcpServer.OnLogMessage += LogMessage;
                
                await _mcpServer.StartAsync(port);
                _isServerRunning = true;
                LogMessage($"MCP服务器已在端口 {port} 上启动");
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
    }
}