using System.Security.Claims;

namespace AIChatWebServer.Services.Context.Interfaces
{
    public interface IUserContextAccessor
    {
        ClaimsPrincipal? User { get; }
        HttpContext? HttpContext { get; }
    }
}