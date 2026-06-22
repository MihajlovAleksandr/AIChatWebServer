using Microsoft.Extensions.Logging;
using AIChatWebServer.Repositories.Interfaces;
using System.Runtime.CompilerServices;

namespace AIChatWebServer.Services.Implementations.Utils
{
    public class CustomLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly ILogsRepository _logsRepository;

        public CustomLogger(string categoryName, ILogsRepository logsRepository)
        {
            _categoryName = categoryName;
            _logsRepository = logsRepository;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return default!;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var message = formatter(state, exception);
            if (exception != null)
            {
                message += $"\nException: {exception.Message}\nStack Trace: {exception.StackTrace}";
            }

            var level = logLevel.ToString();
            var timestamp = DateTime.UtcNow;

            var consoleColor = GetConsoleColor(logLevel);
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = consoleColor;
            Console.WriteLine($"[{timestamp:yyyy-MM-dd HH:mm:ss}] [{level}] [{_categoryName}] {message}");
            Console.ForegroundColor = originalColor;

            Task.Run(async () =>
            {
                try
                {
                    await _logsRepository.Add(level, message, _categoryName, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Failed to write log to database: {ex.Message}");
                }
            });
        }

        private ConsoleColor GetConsoleColor(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace => ConsoleColor.DarkGray,
                LogLevel.Debug => ConsoleColor.Gray,
                LogLevel.Information => ConsoleColor.Green,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Critical => ConsoleColor.DarkRed,
                _ => ConsoleColor.White
            };
        }
    }
}