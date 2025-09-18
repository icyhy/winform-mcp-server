using Microsoft.Extensions.Logging;

namespace WinFormMcpServer;

public class ConsoleLogger<T> : ILogger<T>
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{logLevel}] {typeof(T).Name}: {message}");
        
        if (exception != null)
        {
            Console.WriteLine($"Exception: {exception}");
        }
    }
}