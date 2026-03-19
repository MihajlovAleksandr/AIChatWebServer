using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Exceptions.Implementations.User;
using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Middlewares
{
    public class PremiumRequiredExceptionMiddleware(
        RequestDelegate next,
        ILogger<PremiumRequiredExceptionMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<PremiumRequiredExceptionMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (PremiumRequiredException exception)
            {
                _logger.LogWarning(
                    exception,
                    "API exception: {Message}",
                    exception.Message);

                context.Response.Clear();
                context.Response.StatusCode = exception.StatusCode;
                context.Response.ContentType = "application/json";

                var response = ApiError.Create(
                    exception.ErrorCode,
                    new PremiumFeatureResponse(exception.Feature));

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}