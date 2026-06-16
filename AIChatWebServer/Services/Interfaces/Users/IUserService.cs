using AIChatWebServer.Models.User;

namespace AIChatWebServer.Services.Interfaces.Users
{
    public interface IUserService
    {
        Task UpdateUserData(Guid userId, UserData userData, CancellationToken cancellationToken);
        Task UpdatePreference(Guid userId, Preference preference, CancellationToken cancellationToken);
        Task<bool> IsPremium(Guid userId, CancellationToken cancellationToken);
        Task<User> GetByIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<UserInfo> GetUserInfo(Guid userId, CancellationToken ct);
        Task<Region> GetRegionByCode(string regionCode,  CancellationToken ct = default);
        Task UpdateSecret(Guid userId, string? identifier, string currentAuthProviderCode, string currentSecret, string newAuthProviderCode, string newSecret, CancellationToken cancellationToken);
        Task<IReadOnlyList<User>> GetByAuthProviderCode(string providerCode, CancellationToken cancellationToken);
        Task<bool> DeleteAuthIdentityAsync(Guid userId, string providerCode, CancellationToken ct = default);
        Task UpsertUserLanguageAsync(Guid userId, LanguageContext context, string languageCode, CancellationToken ct = default);
        Task<string> GetUserLanguageAsync(Guid userId, LanguageContext context, CancellationToken ct = default);
        Task AddAuthIdentity(Guid userId, string identifier, string providerCode, string? secret, CancellationToken ct = default);
    }
}
