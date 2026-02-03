using AIChatWebServer.Models.Exceptions;
using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Middlewares
{
    public sealed class TokenExceptionMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AuthTokenException ex)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var response =
                    ApiError.Create(ex.Error);

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
