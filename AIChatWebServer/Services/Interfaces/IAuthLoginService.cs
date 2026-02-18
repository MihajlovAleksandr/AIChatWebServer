using AIChatWebServer.Models.User;

namespace AIChatWebServer.Services.Interfaces
{
    public interface IAuthLoginService
    {
        Task<User> LoginAsync(
            string identifier,
            string secret,
            string providerCode,
            CancellationToken ct = default);
    }
}
