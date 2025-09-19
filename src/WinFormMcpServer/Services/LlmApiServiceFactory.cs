using WinFormMcpServer.Models;

namespace WinFormMcpServer.Services;

/// <summary>
/// LLM API服务工厂
/// </summary>
public class LlmApiServiceFactory
{
    private readonly LlmApiConfigService _configService;
    private ILlmApiService? _currentService;
    private bool _isCurrentServiceMock;

    public LlmApiServiceFactory(LlmApiConfigService configService)
    {
        _configService = configService ?? throw new ArgumentNullException(nameof(configService));
        
        // 订阅配置变更事件
        _configService.ConfigChanged += OnConfigChanged;
    }

    /// <summary>
    /// 配置变更事件处理
    /// </summary>
    private void OnConfigChanged(object? sender, LlmApiConfig config)
    {
        // 配置变更时，清除当前服务实例，下次获取时重新创建
        if (_currentService is IDisposable disposable)
        {
            disposable.Dispose();
        }
        _currentService = null;
    }

    /// <summary>
    /// 获取LLM API服务实例
    /// </summary>
    /// <returns>LLM API服务实例</returns>
    public ILlmApiService GetService()
    {
        var config = _configService.GetConfig();
        
        // 如果当前服务存在且类型匹配，直接返回
        if (_currentService != null && _isCurrentServiceMock == config.UseMockApi)
        {
            return _currentService;
        }

        // 释放旧服务
        if (_currentService is IDisposable disposable)
        {
            disposable.Dispose();
        }

        // 根据配置创建新服务
        if (config.UseMockApi)
        {
            _currentService = new MockLlmApiService(_configService);
            _isCurrentServiceMock = true;
        }
        else
        {
            _currentService = new RealLlmApiService(_configService);
            _isCurrentServiceMock = false;
        }

        return _currentService;
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        _configService.ConfigChanged -= OnConfigChanged;
        
        if (_currentService is IDisposable disposable)
        {
            disposable.Dispose();
        }
        _currentService = null;
    }
}