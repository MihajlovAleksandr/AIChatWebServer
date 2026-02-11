using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Exceptions.Implementations.Auth;
using AIChatWebServer.Models.User;
using AIChatWebServer.Utils.Errors;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Middlewares
{
    public class UserBanExceptionMiddleware(
        RequestDelegate next,
        ILogger<UserBanExceptionMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<UserBanExceptionMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UserBannedException exception)
            {
                _logger.LogWarning(
                    exception,
                    "API exception: {Message}",
                    exception.Message);

                var mapper = context.RequestServices
                    .GetRequiredService<IResponseMapper<UserBan, BanResponse>>();

                context.Response.Clear();
                context.Response.StatusCode = exception.StatusCode;
                context.Response.ContentType = "application/json";

                var response = ApiError.Create(
                    exception.ErrorCode,
                    mapper.ToResponse(exception.UserBan));

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
