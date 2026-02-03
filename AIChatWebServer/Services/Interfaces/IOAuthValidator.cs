using AIChatWebServer.Models.User;

namespace AIChatWebServer.Services.Interfaces
{
    public interface IOAuthValidator
    {
        Task<OAuthUser?> ValidateAsync(string authToken);
    }
}
