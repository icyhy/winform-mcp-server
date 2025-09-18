using System.Text.Json;
using WinFormMcpServer.Models;
using WinFormMcpServer.Services;
using System.Text.Json;

namespace WinFormMcpServer
{
    /// <summary>
    /// 工具列表显示窗体
    /// </summary>
    public partial class ToolsListForm : Form
    {
        private readonly IMcpClientService _mcpClientService;
        private readonly string _serverName;
        private List<RemoteToolInfo> _tools = new();

        public ToolsListForm(IMcpClientService mcpClientService, string serverName)
        {
            InitializeComponent();
            _mcpClientService = mcpClientService ?? throw new ArgumentNullException(nameof(mcpClientService));
            _serverName = serverName ?? throw new ArgumentNullException(nameof(serverName));
            
            this.Text = $"工具列表 - {serverName}";
            
            // 加载工具列表
            _ = LoadToolsAsync();
        }

        private async Task LoadToolsAsync()
        {
            try
        {
                lblStatus.Text = "正在加载工具列表...";
                lblStatus.ForeColor = Color.Blue;
                
                _tools = await _mcpClientService.GetServerToolsAsync(_serverName);
                
                listViewTools.Items.Clear();
                
                if (_tools.Count == 0)
                {
                    lblStatus.Text = "该服务器没有可用的工具";
                    lblStatus.ForeColor = Color.Orange;
                    btnTestTool.Enabled = false;
                    return;
                }
                
                foreach (var tool in _tools)
                {
                    var item = new ListViewItem(tool.Name);
                    item.SubItems.Add(tool.Description);
                    item.Tag = tool;
                    listViewTools.Items.Add(item);
                }
                
                lblStatus.Text = $"找到 {_tools.Count} 个工具";
                lblStatus.ForeColor = Color.Green;
                btnTestTool.Enabled = false; // 需要选择工具后才能测试
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"加载失败: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                btnTestTool.Enabled = false;
            }
        }

        private void listViewTools_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewTools.SelectedItems.Count > 0)
            {
                var tool = listViewTools.SelectedItems[0].Tag as RemoteToolInfo;
                if (tool != null)
                {
                    ShowToolDetails(tool);
                    btnTestTool.Enabled = true;
                }
            }
            else
            {
                ClearToolDetails();
                btnTestTool.Enabled = false;
            }
        }

        private void ShowToolDetails(RemoteToolInfo tool)
        {
            txtToolName.Text = tool.Name;
            txtToolDescription.Text = tool.Description;
            txtServerName.Text = tool.ServerName;
            
            // 显示输入架构
            if (tool.InputSchema != null)
            {
                try
                {
                    var schemaJson = JsonSerializer.Serialize(tool.InputSchema, new JsonSerializerOptions { WriteIndented = true });
                    txtInputSchema.Text = schemaJson;
                }
                catch
                {
                    txtInputSchema.Text = tool.InputSchema.ToString() ?? "";
                }
            }
            else
            {
                txtInputSchema.Text = "无输入参数";
            }
        }

        private void ClearToolDetails()
        {
            txtToolName.Clear();
            txtToolDescription.Clear();
            txtServerName.Clear();
            txtInputSchema.Clear();
        }

        private async void btnTestTool_Click(object sender, EventArgs e)
        {
            if (listViewTools.SelectedItems.Count == 0) return;

            var tool = listViewTools.SelectedItems[0].Tag as RemoteToolInfo;
            if (tool == null) return;

            try
            {
                btnTestTool.Enabled = false;
                btnTestTool.Text = "测试中...";

                // 调用工具
                var result = await _mcpClientService.CallToolAsync(_serverName, tool.Name, new Dictionary<string, object>());

                // 将结果序列化为JSON字符串
                var resultJson = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });

                // 显示结果
                var resultForm = new ToolTestResultForm(tool.Name, resultJson);
                resultForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"测试工具失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnTestTool.Enabled = true;
                btnTestTool.Text = "测试工具";
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadToolsAsync();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}