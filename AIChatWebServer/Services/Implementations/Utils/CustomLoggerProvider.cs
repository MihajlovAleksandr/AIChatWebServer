using Microsoft.Extensions.Logging;
using AIChatWebServer.Repositories.Interfaces;

namespace AIChatWebServer.Services.Implementations.Utils
{
    public class CustomLoggerProvider : ILoggerProvider
    {
        private readonly ILogsRepository _logsRepository;

        public CustomLoggerProvider(ILogsRepository logsRepository)
        {
            _logsRepository = logsRepository;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new CustomLogger(categoryName, _logsRepository);
        }

        public void Dispose()
        {
        }
    }
}