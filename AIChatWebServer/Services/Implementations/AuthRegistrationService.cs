using AIChatWebServer.Models.Exceptions.Implementations.Auth.Register;
using AIChatWebServer.Models.User;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces;
using Npgsql;
using System.Transactions;

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

        public async Task MarkEmailVerifiedAsync(
            Guid userId,
            CancellationToken ct = default)
        {
            using var scope =
                new TransactionScope(
                    TransactionScopeAsyncFlowOption.Enabled);

            await _repository.UpdateRegistrationStateAsync(
                userId,
                RegistrationState.EmailVerified,
                ct);

            scope.Complete();
        }

        public async Task AddUserDataAsync(
            Guid userId,
            UserData data,
            CancellationToken ct = default)
        {
            using var scope =
                new TransactionScope(
                    TransactionScopeAsyncFlowOption.Enabled);

            await _repository.SaveUserDataAsync(
                userId,
                data,
                ct);

            await _repository.UpdateRegistrationStateAsync(
                userId,
                RegistrationState.UserDataCompleted,
                ct);

            scope.Complete();
        }
        public async Task AddPreferenceAsync(
            Guid userId,
            Preference preference,
            CancellationToken ct = default)
        {
            using var scope =
                new TransactionScope(
                    TransactionScopeAsyncFlowOption.Enabled);

            await _repository.SavePreferenceAsync(
                userId,
                preference,
                ct);

            await _repository.UpdateRegistrationStateAsync(
                userId,
                RegistrationState.PreferenceCompleted,
                ct);

            scope.Complete();
        }

        public async Task CompleteRegistrationAsync(
            Guid userId,
            CancellationToken ct = default)
        {
            using var scope =
                new TransactionScope(
                    TransactionScopeAsyncFlowOption.Enabled);

            await _repository.UpdateRegistrationStateAsync(
                userId,
                RegistrationState.Completed,
                ct);

            scope.Complete();
        }

        public Task<RegistrationState> GetRegistrationStateAsync(
            Guid userId, CancellationToken ct = default) =>
            _repository.GetRegistrationStateAsync(userId, ct);
    }
}
