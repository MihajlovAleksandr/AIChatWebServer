using AIChatWebServer.Models.Exceptions.Implementations;
using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Middlewares
{
    public sealed class ApiExceptionMiddleware(
        RequestDelegate next,
        ILogger<ApiExceptionMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ApiExceptionMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ApiExceptionBase ex)
            {
                await HandleApiExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                await HandleUnknownExceptionAsync(context, ex);
            }
        }

        private async Task HandleApiExceptionAsync(
            HttpContext context,
            ApiExceptionBase exception)
        {
            var endpointPath = $"{context.Request.Method} {context.Request.Path}{context.Request.QueryString}";

            _logger.LogWarning(
                exception,
                "API exception at endpoint {Endpoint}: {Message}",
                endpointPath,
                exception.Message);

            context.Response.Clear();
            context.Response.StatusCode = exception.StatusCode;
            context.Response.ContentType = "application/json";

            var response = ApiError.Create(exception.ErrorCode);

            await context.Response.WriteAsJsonAsync(response);
        }

        private async Task HandleUnknownExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            var endpointPath = $"{context.Request.Method} {context.Request.Path}{context.Request.QueryString}";

            _logger.LogError(
                exception,
                "Unhandled exception at endpoint {Endpoint}",
                endpointPath);

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var response = ApiError.Create(
                CommonErrors.InternalError);

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}