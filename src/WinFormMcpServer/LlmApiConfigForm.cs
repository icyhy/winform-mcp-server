using WinFormMcpServer.Models;
using WinFormMcpServer.Services;

namespace WinFormMcpServer;

/// <summary>
/// LLM API配置窗体
/// </summary>
public partial class LlmApiConfigForm : Form
{
    private readonly LlmApiConfigService _configService;
    private LlmApiConfig _currentConfig;

    public LlmApiConfigForm(LlmApiConfigService configService)
    {
        InitializeComponent();
        _configService = configService ?? throw new ArgumentNullException(nameof(configService));
        _currentConfig = _configService.GetConfig();
        
        LoadConfigToUI();
    }

    /// <summary>
    /// 将配置加载到UI
    /// </summary>
    private void LoadConfigToUI()
    {
        radioButtonMockApi.Checked = _currentConfig.UseMockApi;
        radioButtonRealApi.Checked = !_currentConfig.UseMockApi;
        
        textBoxBaseUrl.Text = _currentConfig.BaseUrl;
        textBoxApiKey.Text = _currentConfig.ApiKey;
        textBoxModelName.Text = _currentConfig.ModelName;
        numericUpDownTimeout.Value = _currentConfig.TimeoutSeconds;
        numericUpDownMaxTokens.Value = _currentConfig.MaxTokens;
        numericUpDownTemperature.Value = (decimal)_currentConfig.Temperature;
        
        UpdateUIState();
    }

    /// <summary>
    /// 从UI获取配置
    /// </summary>
    /// <returns>配置对象</returns>
    private LlmApiConfig GetConfigFromUI()
    {
        return new LlmApiConfig
        {
            UseMockApi = radioButtonMockApi.Checked,
            BaseUrl = textBoxBaseUrl.Text.Trim(),
            ApiKey = textBoxApiKey.Text.Trim(),
            ModelName = textBoxModelName.Text.Trim(),
            TimeoutSeconds = (int)numericUpDownTimeout.Value,
            MaxTokens = (int)numericUpDownMaxTokens.Value,
            Temperature = (double)numericUpDownTemperature.Value
        };
    }

    /// <summary>
    /// 更新UI状态
    /// </summary>
    private void UpdateUIState()
    {
        bool isRealApi = radioButtonRealApi.Checked;
        groupBoxApiSettings.Enabled = isRealApi;
        buttonTest.Enabled = isRealApi;
    }

    /// <summary>
    /// 真实API单选按钮状态改变事件
    /// </summary>
    private void radioButtonRealApi_CheckedChanged(object sender, EventArgs e)
    {
        UpdateUIState();
    }

    /// <summary>
    /// 确定按钮点击事件
    /// </summary>
    private void buttonOK_Click(object sender, EventArgs e)
    {
        var config = GetConfigFromUI();
        
        // 验证配置
        var (isValid, errorMessage) = _configService.ValidateConfig(config);
        if (!isValid)
        {
            MessageBox.Show($"配置验证失败：{errorMessage}", "错误", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // 保存配置
        if (_configService.SaveConfig(config))
        {
            MessageBox.Show("配置保存成功！", "成功", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
        else
        {
            MessageBox.Show("配置保存失败，请检查文件权限。", "错误", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// 测试连接按钮点击事件
    /// </summary>
    private async void buttonTest_Click(object sender, EventArgs e)
    {
        if (radioButtonMockApi.Checked)
        {
            MessageBox.Show("Mock API 无需测试连接。", "提示", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var config = GetConfigFromUI();
        
        // 验证配置
        var (isValid, errorMessage) = _configService.ValidateConfig(config);
        if (!isValid)
        {
            MessageBox.Show($"配置验证失败：{errorMessage}", "错误", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        buttonTest.Enabled = false;
        buttonTest.Text = "测试中...";

        try
        {
            // 这里可以实现实际的API测试逻辑
            await TestApiConnection(config);
            
            MessageBox.Show("API连接测试成功！", "成功", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"API连接测试失败：{ex.Message}", "错误", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            buttonTest.Enabled = true;
            buttonTest.Text = "测试连接";
        }
    }

    /// <summary>
    /// 重置按钮点击事件
    /// </summary>
    private void buttonReset_Click(object sender, EventArgs e)
    {
        var result = MessageBox.Show("确定要重置为默认配置吗？", "确认", 
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        
        if (result == DialogResult.Yes)
        {
            _currentConfig = new LlmApiConfig();
            LoadConfigToUI();
        }
    }

    /// <summary>
    /// 测试API连接
    /// </summary>
    /// <param name="config">配置</param>
    /// <returns>测试任务</returns>
    private async Task TestApiConnection(LlmApiConfig config)
    {
        using var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds);
        
        // 添加认证头
        if (!string.IsNullOrWhiteSpace(config.ApiKey))
        {
            httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", config.ApiKey);
        }

        // 构建测试请求
        var requestBody = new
        {
            model = config.ModelName,
            messages = new[]
            {
                new { role = "user", content = "Hello, this is a test message." }
            },
            max_tokens = Math.Min(config.MaxTokens, 10),
            temperature = config.Temperature
        };

        var json = System.Text.Json.JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        // 发送请求
        var response = await httpClient.PostAsync($"{config.BaseUrl.TrimEnd('/')}/chat/completions", content);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"HTTP {response.StatusCode}: {errorContent}");
        }

        // 简单验证响应格式
        var responseContent = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseContent))
        {
            throw new Exception("API返回空响应");
        }

        // 可以进一步验证响应JSON格式
        try
        {
            using var jsonDoc = System.Text.Json.JsonDocument.Parse(responseContent);
            if (!jsonDoc.RootElement.TryGetProperty("choices", out _))
            {
                throw new Exception("API响应格式不正确，缺少choices字段");
            }
        }
        catch (System.Text.Json.JsonException)
        {
            throw new Exception("API响应不是有效的JSON格式");
        }
    }
}