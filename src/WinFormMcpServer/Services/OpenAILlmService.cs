using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace WinFormMcpServer.Services;

public class OpenAILlmService : ILlmService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenAILlmService> _logger;
    private readonly string _apiKey;
    private readonly string _model;

    public OpenAILlmService(HttpClient httpClient, ILogger<OpenAILlmService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI API Key not configured");
        _model = configuration["OpenAI:Model"] ?? "gpt-3.5-turbo";
        
        _httpClient.BaseAddress = new Uri("https://api.openai.com/");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    public async Task<string> GenerateResponseAsync(string systemPrompt, string userMessage)
    {
        try
        {
            var request = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userMessage }
                },
                max_tokens = 1000,
                temperature = 0.7
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("发送OpenAI请求: {Model}", _model);
            
            var response = await _httpClient.PostAsync("v1/chat/completions", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObj = JsonSerializer.Deserialize<JsonElement>(responseJson);

            var messageContent = responseObj
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return messageContent ?? "抱歉，我无法生成回复。";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenAI API调用失败");
            return "抱歉，AI服务暂时不可用，请稍后再试。";
        }
    }

    public async Task<string> GenerateResponseWithToolResultsAsync(string systemPrompt, string userMessage, string toolResults)
    {
        try
        {
            var enhancedPrompt = $"{systemPrompt}\n\n工具执行结果:\n{toolResults}\n\n请基于以上工具执行结果，为用户提供完整的回复。";
            
            var request = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = enhancedPrompt },
                    new { role = "user", content = userMessage }
                },
                max_tokens = 1000,
                temperature = 0.7
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("发送OpenAI请求（包含工具结果）: {Model}", _model);
            
            var response = await _httpClient.PostAsync("v1/chat/completions", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObj = JsonSerializer.Deserialize<JsonElement>(responseJson);

            var messageContent = responseObj
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return messageContent ?? "抱歉，我无法生成回复。";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenAI API调用失败（工具结果）");
            return "抱歉，AI服务暂时不可用，请稍后再试。";
        }
    }
}