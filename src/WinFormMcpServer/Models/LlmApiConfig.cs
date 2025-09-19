namespace WinFormMcpServer.Models;

/// <summary>
/// LLM API配置模型
/// </summary>
public class LlmApiConfig
{
    /// <summary>
    /// 是否使用Mock API（false表示使用真实API）
    /// </summary>
    public bool UseMockApi { get; set; } = true;

    /// <summary>
    /// API基础URL
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.openai.com/v1";

    /// <summary>
    /// API密钥
    /// </summary>
    public string ApiKey { get; set; } = "";

    /// <summary>
    /// 模型名称
    /// </summary>
    public string ModelName { get; set; } = "gpt-3.5-turbo";

    /// <summary>
    /// 请求超时时间（秒）
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// 最大tokens数
    /// </summary>
    public int MaxTokens { get; set; } = 1000;

    /// <summary>
    /// 温度参数（0-2）
    /// </summary>
    public double Temperature { get; set; } = 0.7;

    /// <summary>
    /// 验证配置是否有效
    /// </summary>
    /// <returns>配置是否有效</returns>
    public bool IsValid()
    {
        if (UseMockApi)
        {
            return true; // Mock API不需要验证
        }

        return !string.IsNullOrWhiteSpace(BaseUrl) &&
               !string.IsNullOrWhiteSpace(ApiKey) &&
               !string.IsNullOrWhiteSpace(ModelName) &&
               TimeoutSeconds > 0 &&
               MaxTokens > 0 &&
               Temperature >= 0 && Temperature <= 2;
    }

    /// <summary>
    /// 克隆配置
    /// </summary>
    /// <returns>配置副本</returns>
    public LlmApiConfig Clone()
    {
        return new LlmApiConfig
        {
            UseMockApi = UseMockApi,
            BaseUrl = BaseUrl,
            ApiKey = ApiKey,
            ModelName = ModelName,
            TimeoutSeconds = TimeoutSeconds,
            MaxTokens = MaxTokens,
            Temperature = Temperature
        };
    }
}