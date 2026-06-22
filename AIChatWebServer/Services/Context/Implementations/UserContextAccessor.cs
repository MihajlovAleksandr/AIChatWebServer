using AIChatWebServer.Services.Context.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace AIChatWebServer.Services.Context.Implementations
{
    internal sealed class UserContextAccessor : IUserContextAccessor
    {
        private static readonly AsyncLocal<HubCallerContext?> _hubContext = new();

        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor
                ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public static void SetHubContext(HubCallerContext context)
        {
            _hubContext.Value = context;
        }

        public static void Clear()
        {
            _hubContext.Value = null;
        }

        public ClaimsPrincipal? User =>
            _hubContext.Value?.User
            ?? _httpContextAccessor.HttpContext?.User;

        public HttpContext? HttpContext =>
            _hubContext.Value?.GetHttpContext()
            ?? _httpContextAccessor.HttpContext;
    }
}