using AIChatWebServer.Models.User;

namespace AIChatWebServer.Services.Interfaces.Auth
{
    public interface IOAuthValidator
    {
        Task<OAuthUser> ValidateAsync(string authToken);
    }
}
