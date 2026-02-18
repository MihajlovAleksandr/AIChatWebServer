using AIChatWebServer.Models.User;

namespace AIChatWebServer.Services.Interfaces
{
    public interface IAuthRegistrationService
    {
        Task<Guid> RegisterAsync(
            string email,
            string secret,
            string providerCode,
            string regionCode,
            string languageCode,
            CancellationToken ct = default);

        Task MarkEmailVerifiedAsync(Guid userId, CancellationToken ct = default);

        Task AddUserDataAsync(Guid userId, UserData data, CancellationToken ct = default);

        Task AddPreferenceAsync(Guid userId, Preference preference, CancellationToken ct = default);

        Task CompleteRegistrationAsync(Guid userId, CancellationToken ct = default);

        Task<RegistrationState> GetRegistrationStateAsync(Guid userId, CancellationToken ct = default);

    }
}
