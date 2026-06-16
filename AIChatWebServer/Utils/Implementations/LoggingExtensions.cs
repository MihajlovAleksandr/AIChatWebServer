using AIChatWebServer.Services.Implementations.Utils;

namespace AIChatWebServer.Extensions
{
    public static class LoggingExtensions
    {
        public static ILoggingBuilder AddCustomLogging(this ILoggingBuilder builder)
        {
            builder.ClearProviders();

            builder.Services.AddSingleton<ILoggerProvider, CustomLoggerProvider>();

            return builder;
        }

        public static IServiceCollection AddCustomLogging(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerProvider, CustomLoggerProvider>();
            return services;
        }
    }
}