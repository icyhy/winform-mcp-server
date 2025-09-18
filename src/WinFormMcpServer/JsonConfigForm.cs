using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using WinFormMcpServer.Services;

namespace WinFormMcpServer
{
    public partial class JsonConfigForm : Form
    {
        private readonly string _configFilePath;
        private readonly IMcpClientService? _mcpClientService;
        private bool _hasUnsavedChanges = false;

        public JsonConfigForm(IMcpClientService? mcpClientService = null)
        {
            InitializeComponent();
            _mcpClientService = mcpClientService;
            _configFilePath = Path.Combine(Application.StartupPath, "mcp-servers.json");
        }

        private void JsonConfigForm_Load(object sender, EventArgs e)
        {
            LoadJsonConfig();
        }

        private void LoadJsonConfig()
        {
            try
            {
                if (File.Exists(_configFilePath))
                {
                    string jsonContent = File.ReadAllText(_configFilePath);
                    // 格式化JSON以便更好地显示
                    var jsonDocument = JsonDocument.Parse(jsonContent);
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    };
                    string formattedJson = JsonSerializer.Serialize(jsonDocument.RootElement, options);
                    textBoxJson.Text = formattedJson;
                    _hasUnsavedChanges = false;
                    UpdateStatus("配置文件加载成功", false);
                }
                else
                {
                    // 创建默认配置
                    CreateDefaultConfig();
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"加载配置文件失败: {ex.Message}", true);
                MessageBox.Show($"加载配置文件失败:\\n{ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateDefaultConfig()
        {
            var defaultConfig = new
            {
                mcpServers = new
                {
                    defaultServer = new
                    {
                        url = "http://localhost:3000/sse",
                        type = "sse",
                        note = "默认MCP服务器",
                        enabled = true,
                        timeout = 30000,
                        retryCount = 3
                    }
                }
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            textBoxJson.Text = JsonSerializer.Serialize(defaultConfig, options);
            _hasUnsavedChanges = true;
            UpdateStatus("已创建默认配置，请保存", false);
        }

        private void textBoxJson_TextChanged(object sender, EventArgs e)
        {
            _hasUnsavedChanges = true;
            UpdateStatus("配置已修改，未保存", false);
        }

        private void buttonValidate_Click(object sender, EventArgs e)
        {
            ValidateJson();
        }

        private bool ValidateJson()
        {
            try
            {
                string jsonText = textBoxJson.Text.Trim();
                if (string.IsNullOrEmpty(jsonText))
                {
                    UpdateStatus("JSON内容不能为空", true);
                    return false;
                }

                // 验证JSON格式
                var jsonDocument = JsonDocument.Parse(jsonText);
                
                // 验证必要的结构
                if (!jsonDocument.RootElement.TryGetProperty("mcpServers", out var serversElement))
                {
                    UpdateStatus("JSON必须包含 'mcpServers' 属性", true);
                    return false;
                }

                if (serversElement.ValueKind != JsonValueKind.Object)
                {
                    UpdateStatus("'mcpServers' 必须是一个对象", true);
                    return false;
                }

                // 验证每个服务器配置
                foreach (var serverProperty in serversElement.EnumerateObject())
                {
                    var serverConfig = serverProperty.Value;
                    
                    // 检查必要属性
                    if (!serverConfig.TryGetProperty("url", out _) && 
                        !serverConfig.TryGetProperty("command", out _))
                    {
                        UpdateStatus($"服务器 '{serverProperty.Name}' 必须包含 'url' 或 'command' 属性", true);
                        return false;
                    }

                    if (!serverConfig.TryGetProperty("type", out var typeElement))
                    {
                        UpdateStatus($"服务器 '{serverProperty.Name}' 必须包含 'type' 属性", true);
                        return false;
                    }

                    string serverType = typeElement.GetString() ?? "";
                    if (!IsValidServerType(serverType))
                    {
                        UpdateStatus($"服务器 '{serverProperty.Name}' 的类型 '{serverType}' 无效", true);
                        return false;
                    }
                }

                UpdateStatus("JSON配置验证通过", false);
                return true;
            }
            catch (JsonException ex)
            {
                UpdateStatus($"JSON格式错误: {ex.Message}", true);
                return false;
            }
            catch (Exception ex)
            {
                UpdateStatus($"验证失败: {ex.Message}", true);
                return false;
            }
        }

        private bool IsValidServerType(string type)
        {
            return type == "sse" || type == "http" || type == "process";
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (!ValidateJson())
            {
                return;
            }

            try
            {
                // 确保目录存在
                string? directory = Path.GetDirectoryName(_configFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // 保存文件
                File.WriteAllText(_configFilePath, textBoxJson.Text);
                _hasUnsavedChanges = false;
                UpdateStatus("配置文件保存成功", false);

                // 通知MCP客户端服务重新加载配置
                if (_mcpClientService != null)
                {
                    try
                    {
                        _mcpClientService.ReloadConfiguration();
                        UpdateStatus("配置文件保存成功，MCP服务已重新加载配置", false);
                    }
                    catch (Exception ex)
                    {
                        UpdateStatus($"配置文件保存成功，但重新加载MCP配置失败: {ex.Message}", true);
                    }
                }

                MessageBox.Show("配置保存成功！", "成功", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // 保存成功后自动关闭窗口
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                UpdateStatus($"保存失败: {ex.Message}", true);
                MessageBox.Show($"保存配置文件失败:\\n{ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (_hasUnsavedChanges)
            {
                var result = MessageBox.Show("有未保存的更改，确定要关闭吗？", "确认", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return;
                }
            }
            
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonReload_Click(object sender, EventArgs e)
        {
            if (_hasUnsavedChanges)
            {
                var result = MessageBox.Show("有未保存的更改，确定要重新加载吗？", "确认", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return;
                }
            }

            LoadJsonConfig();
        }

        private void UpdateStatus(string message, bool isError)
        {
            labelStatus.Text = message;
            labelStatus.ForeColor = isError ? System.Drawing.Color.Red : System.Drawing.Color.Black;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_hasUnsavedChanges && e.CloseReason == CloseReason.UserClosing)
            {
                var result = MessageBox.Show("有未保存的更改，确定要关闭吗？", "确认", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
            
            base.OnFormClosing(e);
        }
    }
}