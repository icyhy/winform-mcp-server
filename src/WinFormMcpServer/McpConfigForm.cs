using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormMcpServer.Models;
using WinFormMcpServer.Services;

namespace WinFormMcpServer
{
    public partial class McpConfigForm : Form
    {
        private readonly IMcpClientService? _mcpClientService;
        private List<McpServerInfo> _servers = new();
        private McpServerInfo? _selectedServer;

        public McpConfigForm(IMcpClientService? mcpClientService)
        {
            InitializeComponent();
            _mcpClientService = mcpClientService;
            
            if (_mcpClientService != null)
            {
                _mcpClientService.ServerStatusChanged += OnServerStatusChanged;
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadServersAsync();
        }

        private async void McpConfigForm_Load(object sender, EventArgs e)
        {
            await LoadServersAsync();
        }

        private async Task LoadServersAsync()
        {
            try
            {
                if (_mcpClientService == null)
                {
                    MessageBox.Show("MCP客户端服务未初始化", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _servers = await _mcpClientService.GetServersAsync();
                UpdateServersList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载服务器列表失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateServersList()
        {
            listViewServers.Items.Clear();
            
            foreach (var server in _servers)
            {
                var item = new ListViewItem(server.Name);
                item.SubItems.Add(server.Config.Type);
                item.SubItems.Add(server.Config.Url ?? "");
                item.SubItems.Add(GetStatusText(server.Status));
                item.SubItems.Add(server.Config.Enabled ? "是" : "否");
                item.Tag = server;
                
                // 根据状态设置颜色
                switch (server.Status)
                {
                    case McpServerStatus.Connected:
                        item.ForeColor = Color.Green;
                        break;
                    case McpServerStatus.Connecting:
                        item.ForeColor = Color.Orange;
                        break;
                    case McpServerStatus.Error:
                    case McpServerStatus.Timeout:
                        item.ForeColor = Color.Red;
                        break;
                    default:
                        item.ForeColor = Color.Black;
                        break;
                }
                
                listViewServers.Items.Add(item);
            }
        }

        private string GetStatusText(McpServerStatus status)
        {
            return status switch
            {
                McpServerStatus.Disconnected => "未连接",
                McpServerStatus.Connecting => "连接中",
                McpServerStatus.Connected => "已连接",
                McpServerStatus.Error => "错误",
                McpServerStatus.Timeout => "超时",
                _ => "未知"
            };
        }

        private void listViewServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewServers.SelectedItems.Count > 0)
            {
                _selectedServer = (McpServerInfo)listViewServers.SelectedItems[0].Tag;
                UpdateServerDetails();
                UpdateToolsList();
            }
            else
            {
                _selectedServer = null;
                ClearServerDetails();
                ClearToolsList();
            }
        }

        private void UpdateServerDetails()
        {
            if (_selectedServer == null) return;

            txtServerName.Text = _selectedServer.Name;
            txtServerType.Text = _selectedServer.Config.Type;
            txtServerUrl.Text = _selectedServer.Config.Url ?? "";
            txtServerNote.Text = _selectedServer.Config.Note ?? "";
            chkServerEnabled.Checked = _selectedServer.Config.Enabled;
            
            lblStatusValue.Text = GetStatusText(_selectedServer.Status);
            lblStatusValue.ForeColor = _selectedServer.Status == McpServerStatus.Connected ? Color.Green : Color.Red;
            
            // 更新按钮状态
            btnConnect.Enabled = _selectedServer.Status != McpServerStatus.Connected && _selectedServer.Status != McpServerStatus.Connecting;
            btnDisconnect.Enabled = _selectedServer.Status == McpServerStatus.Connected;
        }

        private void ClearServerDetails()
        {
            txtServerName.Clear();
            txtServerType.Clear();
            txtServerUrl.Clear();
            txtServerNote.Clear();
            chkServerEnabled.Checked = false;
            lblStatusValue.Text = "未选择";
            lblStatusValue.ForeColor = Color.Gray;
            
            btnConnect.Enabled = false;
            btnDisconnect.Enabled = false;
        }

        private async void UpdateToolsList()
        {
            listViewTools.Items.Clear();
            btnTestTool.Enabled = false;
            
            if (_selectedServer == null || _mcpClientService == null) return;

            try
            {
                var tools = await _mcpClientService.GetServerToolsAsync(_selectedServer.Name);
                
                foreach (var tool in tools)
                {
                    var item = new ListViewItem(tool.Name);
                    item.SubItems.Add(tool.Description);
                    item.Tag = tool;
                    listViewTools.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"获取工具列表失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ClearToolsList()
        {
            listViewTools.Items.Clear();
            btnTestTool.Enabled = false;
        }

        private void listViewTools_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnTestTool.Enabled = listViewTools.SelectedItems.Count > 0 && 
                                  _selectedServer?.Status == McpServerStatus.Connected;
        }

        private async void btnConnectAll_Click(object sender, EventArgs e)
        {
            if (_mcpClientService == null) return;

            try
            {
                btnConnectAll.Enabled = false;
                btnConnectAll.Text = "连接中...";
                
                await _mcpClientService.ConnectAllServersAsync();
                
                MessageBox.Show("已尝试连接所有启用的服务器", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"连接所有服务器失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnConnectAll.Enabled = true;
                btnConnectAll.Text = "连接所有";
            }
        }

        private async void btnDisconnectAll_Click(object sender, EventArgs e)
        {
            if (_mcpClientService == null) return;

            try
            {
                btnDisconnectAll.Enabled = false;
                btnDisconnectAll.Text = "断开中...";
                
                await _mcpClientService.DisconnectAllServersAsync();
                
                MessageBox.Show("已断开所有服务器连接", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"断开所有连接失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnDisconnectAll.Enabled = true;
                btnDisconnectAll.Text = "断开所有";
            }
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            if (_selectedServer == null || _mcpClientService == null) return;

            try
            {
                btnConnect.Enabled = false;
                btnConnect.Text = "连接中...";
                
                var success = await _mcpClientService.ConnectServerAsync(_selectedServer.Name);
                
                if (success)
                {
                    MessageBox.Show($"成功连接到服务器 {_selectedServer.Name}", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"连接服务器 {_selectedServer.Name} 失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"连接服务器失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnConnect.Text = "连接";
                // 刷新按钮状态
				UpdateServerDetails(); 
				// 重新加载工具列表
				UpdateToolsList();

            }
        }

        private async void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (_selectedServer == null || _mcpClientService == null) return;

            try
            {
                btnDisconnect.Enabled = false;
                btnDisconnect.Text = "断开中...";
                
                await _mcpClientService.DisconnectServerAsync(_selectedServer.Name);
                
                MessageBox.Show($"已断开服务器 {_selectedServer.Name} 连接", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"断开连接失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnDisconnect.Text = "断开";
                UpdateServerDetails(); // 刷新按钮状态
            }
        }

        private void btnTestTool_Click(object sender, EventArgs e)
        {
            if (listViewTools.SelectedItems.Count == 0)
            {
                MessageBox.Show("请先选择一个工具", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_selectedServer?.Status != McpServerStatus.Connected)
            {
                MessageBox.Show("服务器未连接", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedTool = (RemoteToolInfo)listViewTools.SelectedItems[0].Tag;
            
            // 调用工具测试
            _ = Task.Run(async () =>
            {
                try
                {
                    this.Invoke(() =>
                    {
                        btnTestTool.Enabled = false;
                        btnTestTool.Text = "测试中...";
                    });

                    var result = await _mcpClientService.CallToolAsync(_selectedServer.Name, selectedTool.Name, new Dictionary<string, object>());
                    var resultJson = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });

                    this.Invoke(() =>
                    {
                        var resultForm = new ToolTestResultForm(selectedTool.Name, resultJson);
                        resultForm.ShowDialog(this);
                    });
                }
                catch (Exception ex)
                {
                    this.Invoke(() =>
                    {
                        MessageBox.Show($"测试工具失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
                finally
                {
                    this.Invoke(() =>
                    {
                        btnTestTool.Enabled = listViewTools.SelectedItems.Count > 0;
                        btnTestTool.Text = "测试工具";
                    });
                }
            });
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void chkServerEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (_selectedServer == null || _mcpClientService == null) return;

            try
            {
                // 更新服务器配置中的启用状态
                _selectedServer.Config.Enabled = chkServerEnabled.Checked;
                
                // 保存配置到文件
                await SaveServerConfigAsync();
                
                // 如果启用了服务器且当前未连接，则自动连接
                if (chkServerEnabled.Checked && _selectedServer.Status != McpServerStatus.Connected)
                {
                    var success = await _mcpClientService.ConnectServerAsync(_selectedServer.Name);
                    if (!success)
                    {
                        MessageBox.Show($"自动连接服务器 {_selectedServer.Name} 失败", "警告", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                // 如果禁用了服务器且当前已连接，则自动断开
                else if (!chkServerEnabled.Checked && _selectedServer.Status == McpServerStatus.Connected)
                {
                    await _mcpClientService.DisconnectServerAsync(_selectedServer.Name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新服务器启用状态失败: {ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                // 恢复原来的状态
                chkServerEnabled.Checked = _selectedServer.Config.Enabled;
            }
        }

        private async Task SaveServerConfigAsync()
        {
            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mcp-servers.json");
                
                // 读取当前配置
                string jsonContent = await File.ReadAllTextAsync(configPath);
                using var document = JsonDocument.Parse(jsonContent);
                var root = document.RootElement;
                
                // 创建新的配置对象
                var configDict = new Dictionary<string, object>();
                
                if (root.TryGetProperty("mcpServers", out var serversElement))
                {
                    var servers = new Dictionary<string, object>();
                    
                    foreach (var serverProperty in serversElement.EnumerateObject())
                    {
                        if (serverProperty.Name == _selectedServer.Name)
                        {
                            // 更新当前服务器的配置
                            servers[serverProperty.Name] = new
                            {
                                url = _selectedServer.Config.Url,
                                type = _selectedServer.Config.Type,
                                note = _selectedServer.Config.Note,
                                enabled = _selectedServer.Config.Enabled,
                                timeout = _selectedServer.Config.Timeout,
                                retryCount = _selectedServer.Config.RetryCount
                            };
                        }
                        else
                        {
                            // 保持其他服务器的配置不变
                            servers[serverProperty.Name] = JsonSerializer.Deserialize<object>(serverProperty.Value.GetRawText());
                        }
                    }
                    
                    configDict["mcpServers"] = servers;
                }
                
                // 保存配置
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                
                string updatedJson = JsonSerializer.Serialize(configDict, options);
                await File.WriteAllTextAsync(configPath, updatedJson);
                
                // 重新加载配置
                _mcpClientService.ReloadConfiguration();
            }
            catch (Exception ex)
            {
                throw new Exception($"保存配置失败: {ex.Message}");
            }
        }

        private void btnManualConfig_Click(object sender, EventArgs e)
        {
            try
            {
                var jsonConfigForm = new JsonConfigForm(_mcpClientService);
                var result = jsonConfigForm.ShowDialog(this);
                
                if (result == DialogResult.OK)
                {
                    // 配置已保存，重新加载服务器列表
                    _ = LoadServersAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开手动配置窗口失败: {ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnServerStatusChanged(object? sender, McpServerStatusChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnServerStatusChanged(sender, e)));
                return;
            }

            // 更新服务器状态
            var server = _servers.FirstOrDefault(s => s.Name == e.ServerName);
            if (server != null)
            {
                server.Status = e.NewStatus;
                server.ErrorMessage = e.ErrorMessage;
                
                UpdateServersList();
                
                if (_selectedServer?.Name == e.ServerName)
                {
                    UpdateServerDetails();
                    // 更新测试工具按钮状态
                    btnTestTool.Enabled = _selectedServer.Status == McpServerStatus.Connected;
                }
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_mcpClientService != null)
            {
                _mcpClientService.ServerStatusChanged -= OnServerStatusChanged;
            }
            base.OnFormClosed(e);
        }
    }
}