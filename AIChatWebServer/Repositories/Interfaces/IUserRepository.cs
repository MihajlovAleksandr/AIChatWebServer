using AIChatWebServer.Models.User;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<Guid> CreateUserAsync(
            string email,
            string regionCode,
            string languageCode,
            AuthIdentity authIdentity,
            CancellationToken cancellationToken = default);

        Task SaveUserDataAsync(
            Guid userId,
            UserData userData,
            CancellationToken cancellationToken = default);

        Task SavePreferenceAsync(
            Guid userId,
            Preference preference,
            CancellationToken cancellationToken = default);

        Task UpdateRegistrationStateAsync(
            Guid userId,
            RegistrationState state,
            CancellationToken ct = default);

        Task UpdateAsync(
            User user,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<RegistrationState> GetRegistrationStateAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<User?> GetByIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<User?> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default);

        Task<User?> GetByAuthIdentityCodeAsync(
            string providerCode,
            string identifier,
            CancellationToken cancellationToken = default);

        Task<UserBan?> GetUserBanByIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<User>> GetByAuthProviderCodeAsync(
            string providerCode,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<User>> GetUsersInSameChatsAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByEmailAsync(
            string email,
            CancellationToken cancellationToken = default);

        Task<Region> GetRegionByCodeAsync(
            string regionCode,
            CancellationToken cancellationToken = default);
    }
}
