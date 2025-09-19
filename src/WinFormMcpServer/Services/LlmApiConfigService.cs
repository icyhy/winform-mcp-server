using System.Text.Json;
using WinFormMcpServer.Models;

namespace WinFormMcpServer.Services;

/// <summary>
/// LLM API配置管理服务
/// </summary>
public class LlmApiConfigService
{
    private const string ConfigFileName = "llm-api-config.json";
    private readonly string _configFilePath;
    private LlmApiConfig? _currentConfig;
    private readonly object _lock = new();

    public LlmApiConfigService()
    {
        _configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName);
    }

    /// <summary>
    /// 获取当前配置
    /// </summary>
    /// <returns>LLM API配置</returns>
    public LlmApiConfig GetConfig()
    {
        lock (_lock)
        {
            if (_currentConfig == null)
            {
                LoadConfig();
            }
            return _currentConfig?.Clone() ?? new LlmApiConfig();
        }
    }

    /// <summary>
    /// 保存配置
    /// </summary>
    /// <param name="config">要保存的配置</param>
    /// <returns>是否保存成功</returns>
    public bool SaveConfig(LlmApiConfig config)
    {
        if (config == null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        try
        {
            lock (_lock)
            {
                var json = JsonSerializer.Serialize(config, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                File.WriteAllText(_configFilePath, json);
                _currentConfig = config.Clone();
                
                // 触发配置变更事件
                ConfigChanged?.Invoke(this, _currentConfig);
                
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"保存LLM API配置失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 加载配置
    /// </summary>
    private void LoadConfig()
    {
        try
        {
            if (File.Exists(_configFilePath))
            {
                var json = File.ReadAllText(_configFilePath);
                var config = JsonSerializer.Deserialize<LlmApiConfig>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                _currentConfig = config ?? new LlmApiConfig();
            }
            else
            {
                _currentConfig = new LlmApiConfig();
                // 保存默认配置
                SaveConfig(_currentConfig);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"加载LLM API配置失败: {ex.Message}");
            _currentConfig = new LlmApiConfig();
        }
    }

    /// <summary>
    /// 重置为默认配置
    /// </summary>
    /// <returns>是否重置成功</returns>
    public bool ResetToDefault()
    {
        var defaultConfig = new LlmApiConfig();
        return SaveConfig(defaultConfig);
    }

    /// <summary>
    /// 验证配置是否有效
    /// </summary>
    /// <param name="config">要验证的配置</param>
    /// <returns>验证结果和错误信息</returns>
    public (bool IsValid, string ErrorMessage) ValidateConfig(LlmApiConfig config)
    {
        if (config == null)
        {
            return (false, "配置不能为空");
        }

        if (!config.UseMockApi)
        {
            if (string.IsNullOrWhiteSpace(config.BaseUrl))
            {
                return (false, "API基础URL不能为空");
            }

            if (string.IsNullOrWhiteSpace(config.ApiKey))
            {
                return (false, "API密钥不能为空");
            }

            if (string.IsNullOrWhiteSpace(config.ModelName))
            {
                return (false, "模型名称不能为空");
            }

            if (config.TimeoutSeconds <= 0)
            {
                return (false, "超时时间必须大于0");
            }

            if (config.MaxTokens <= 0)
            {
                return (false, "最大tokens数必须大于0");
            }

            if (config.Temperature < 0 || config.Temperature > 2)
            {
                return (false, "温度参数必须在0-2之间");
            }

            // 验证URL格式
            if (!Uri.TryCreate(config.BaseUrl, UriKind.Absolute, out _))
            {
                return (false, "API基础URL格式无效");
            }
        }

        return (true, string.Empty);
    }

    /// <summary>
    /// 配置变更事件
    /// </summary>
    public event EventHandler<LlmApiConfig>? ConfigChanged;
}