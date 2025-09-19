using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using WinFormMcpServer.Models;

namespace WinFormMcpServer.Services;

/// <summary>
/// 真实LLM API服务实现（支持OpenAI兼容接口）
/// </summary>
public class RealLlmApiService : ILlmApiService, IDisposable
{
    private readonly LlmApiConfigService _configService;
    private readonly HttpClient _httpClient;
    private bool _disposed = false;

    public RealLlmApiService(LlmApiConfigService configService)
    {
        _configService = configService ?? throw new ArgumentNullException(nameof(configService));
        _httpClient = new HttpClient();
        
        // 订阅配置变更事件
        _configService.ConfigChanged += OnConfigChanged;
        
        // 初始化HTTP客户端
        UpdateHttpClientConfig();
    }

    /// <summary>
    /// 配置变更事件处理
    /// </summary>
    private void OnConfigChanged(object? sender, LlmApiConfig config)
    {
        UpdateHttpClientConfig();
    }

    /// <summary>
    /// 更新HTTP客户端配置
    /// </summary>
    private void UpdateHttpClientConfig()
    {
        var config = _configService.GetConfig();
        
        // 设置超时
        _httpClient.Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds);
        
        // 清除现有的认证头
        _httpClient.DefaultRequestHeaders.Authorization = null;
        
        // 设置认证头
        if (!string.IsNullOrWhiteSpace(config.ApiKey))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", config.ApiKey);
        }
        
        // 设置User-Agent
        _httpClient.DefaultRequestHeaders.UserAgent.Clear();
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("WinFormMcpServer/1.0");
    }

    /// <summary>
    /// 发送聊天消息
    /// </summary>
    /// <param name="messages">消息列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>AI回复</returns>
    public async Task<string> SendChatMessageAsync(IEnumerable<ChatMessage> messages, CancellationToken cancellationToken = default)
    {
        if (messages == null || !messages.Any())
        {
            throw new ArgumentException("消息列表不能为空", nameof(messages));
        }

        var config = _configService.GetConfig();
        if (config.UseMockApi)
        {
            throw new InvalidOperationException("当前配置为Mock API，不应调用真实API服务");
        }

        // 构建请求体
        var requestBody = new
        {
            model = config.ModelName,
            messages = messages.Select(m => new { role = m.Role, content = m.Content }).ToArray(),
            max_tokens = config.MaxTokens,
            temperature = config.Temperature,
            stream = false
        };

        var json = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            // 发送请求
            var url = $"{config.BaseUrl.TrimEnd('/')}/chat/completions";
            var response = await _httpClient.PostAsync(url, content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"API请求失败 (HTTP {response.StatusCode}): {errorContent}");
            }

            // 解析响应
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var responseJson = JsonDocument.Parse(responseContent);

            // 提取回复内容
            if (responseJson.RootElement.TryGetProperty("choices", out var choices) &&
                choices.GetArrayLength() > 0)
            {
                var firstChoice = choices[0];
                if (firstChoice.TryGetProperty("message", out var message) &&
                    message.TryGetProperty("content", out var messageContent))
                {
                    return messageContent.GetString() ?? "API返回了空的回复内容";
                }
            }

            throw new InvalidOperationException("API响应格式不正确，无法提取回复内容");
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            throw new TimeoutException($"API请求超时（{config.TimeoutSeconds}秒）", ex);
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw new OperationCanceledException("API请求被取消", cancellationToken);
        }
        catch (HttpRequestException)
        {
            throw; // 重新抛出HTTP异常
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("API响应JSON格式无效", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"API调用失败: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 测试API连接
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>测试结果</returns>
    public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var testMessages = new[]
            {
                new ChatMessage { Role = "user", Content = "Hello, this is a test message." }
            };

            var response = await SendChatMessageAsync(testMessages, cancellationToken);
            return !string.IsNullOrWhiteSpace(response);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 获取当前配置
    /// </summary>
    /// <returns>配置信息</returns>
    public LlmApiConfig GetCurrentConfig()
    {
        return _configService.GetConfig();
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    /// <param name="disposing">是否正在释放</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _configService.ConfigChanged -= OnConfigChanged;
            _httpClient?.Dispose();
            _disposed = true;
        }
    }
}

/// <summary>
/// 自定义JsonNamingPolicy，将PascalCase转换为snake_case
/// </summary>
public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public static SnakeCaseNamingPolicy Instance { get; } = new SnakeCaseNamingPolicy();

    public override string ConvertName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        var result = new StringBuilder();
        for (int i = 0; i < name.Length; i++)
        {
            if (char.IsUpper(name[i]) && i > 0)
            {
                result.Append('_');
            }
            result.Append(char.ToLower(name[i]));
        }
        return result.ToString();
    }
}

/// <summary>
/// JsonSerializerOptions扩展
/// </summary>
public static class JsonSerializerOptionsExtensions
{
    /// <summary>
    /// Snake case lower命名策略
    /// </summary>
    public static JsonNamingPolicy SnakeCaseLower => SnakeCaseNamingPolicy.Instance;
}