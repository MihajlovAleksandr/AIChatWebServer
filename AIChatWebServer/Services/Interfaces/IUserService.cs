using AIChatWebServer.Models.User;

namespace AIChatWebServer.Services.Interfaces
{
    public interface IUserService
    {
        Task UpdateUserData(Guid userId, UserData userData, CancellationToken cancellationToken);
        Task UpdatePreference(Guid userId, Preference preference, CancellationToken cancellationToken);
        Task<bool> IsPremium(Guid userId, CancellationToken cancellationToken);
        Task<User> GetByIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<UserInfo> GetUserInfo(Guid userId, CancellationToken ct);
        Task UpdateSecret(Guid userId, string? identifier, string currentAuthProviderCode, string currentSecret, string newAuthProviderCode, string newSecret, CancellationToken cancellationToken);
    }
}
