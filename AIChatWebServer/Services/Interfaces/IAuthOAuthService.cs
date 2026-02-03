using AIChatWebServer.Models.User;

namespace AIChatWebServer.Services.Interfaces
{
    public interface IAuthOAuthService
    {
        Task<User?> LoginGoogleAsync(
            string email,
            string googleId,
            CancellationToken ct = default);

        Task<Guid> RegisterGoogleAsync(
            string email,
            string googleId,
            string regionCode,
            string languageCode,
            CancellationToken ct = default);
    }
}
