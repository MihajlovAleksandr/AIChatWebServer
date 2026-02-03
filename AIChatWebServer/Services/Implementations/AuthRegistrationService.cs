using AIChatWebServer.Models.Exceptions;
using AIChatWebServer.Models.User;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces;
using Npgsql;

namespace AIChatWebServer.Services.Implementations
{
    public sealed class AuthRegistrationService(
        IUserRepository repository,
        IHasher hasher) : IAuthRegistrationService
    {
        private readonly IUserRepository _repository = repository;
        private readonly IHasher _hasher = hasher;

        public async Task<Guid> RegisterAsync(
            string email,
            string secret,
            string providerCode,
            string regionCode,
            string languageCode,
            CancellationToken ct = default)
        {
            try
            {
                return await _repository.CreateUserAsync(
                    email,
                    regionCode,
                    languageCode,
                    new AuthIdentity(
                        new AuthProvider(providerCode),
                        email,
                        _hasher.Hash(secret)),
                    ct);
            }
            catch (PostgresException ex)
                when (ex.SqlState == "23505")
            {
                throw new UserAlreadyExistsException(email);
            }
        }

        public Task MarkEmailVerifiedAsync(
            Guid userId,
            CancellationToken ct = default) =>
            _repository.MarkEmailVerifiedAsync(userId, ct);

        public Task AddUserDataAsync(
            Guid userId,
            UserData data,
            CancellationToken ct = default) =>
            _repository.SaveUserDataAsync(userId, data, ct);

        public Task AddPreferenceAsync(
            Guid userId,
            Preference preference,
            CancellationToken ct = default) =>
            _repository.SavePreferenceAsync(userId, preference, ct);

        public Task CompleteRegistrationAsync(
            Guid userId,
            CancellationToken ct = default) =>
            _repository.CompleteRegistrationAsync(userId, ct);

        public Task<RegistrationState> GetRegistrationStateAsync(
            Guid userId, CancellationToken ct = default) =>
            _repository.GetRegistrationStateAsync(userId, ct);
    }
}
