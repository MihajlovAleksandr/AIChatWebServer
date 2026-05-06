using AIChatWebServer.Models.Exceptions.Implementations.Auth.Login;
using AIChatWebServer.Models.Exceptions.Implementations.User;
using AIChatWebServer.Models.User;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces;
using ConnectionInfo = AIChatWebServer.Models.Connection.ConnectionInfo;

namespace AIChatWebServer.Services.Implementations
{
    public sealed class UserService(IUserRepository userRepository, IConnectionService connectionService, IHasher hasher) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IConnectionService _connectionService = connectionService;
        private readonly IHasher _hasher = hasher;

        public async Task<bool> IsPremium(Guid userId, CancellationToken cancellationToken)
        {
            User user = await _userRepository.GetByIdAsync(userId, cancellationToken) ?? throw new UserNotFoundException(userId);

            return user.IsPremium();
        }

        public async Task UpdatePreference(Guid userId, Preference preference, CancellationToken cancellationToken)
        {
            await _userRepository.SavePreferenceAsync(userId, preference, cancellationToken);
        }

        public async Task UpdateUserData(Guid userId, UserData userData, CancellationToken cancellationToken)
        {
            await _userRepository.SaveUserDataAsync(userId, userData, cancellationToken);
        }

        public async Task<User> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _userRepository.GetByIdAsync(userId, cancellationToken) ?? throw new UserNotFoundException(userId);
        }

        public async Task UpdateSecret(Guid userId, string? identifier, string currentAuthProviderCode, string currentSecret, string newAuthProviderCode, string newSecret, CancellationToken cancellationToken)
        {
            User user = await GetByIdAsync(userId, cancellationToken);
            var identity =
                user.GetAuthIdentity(currentAuthProviderCode);
            identifier ??= user.Email;

            if (identity?.Secret == null)
                throw new InvalidLoginProviderException(user.Email, currentAuthProviderCode);

            if (!_hasher.Verify(currentSecret, identity.Secret))
                throw new InvalidCredentialsException(user.Id);
            AuthIdentity authIdentity = new AuthIdentity(new AuthProvider(newAuthProviderCode), identifier, newSecret);
            await _userRepository.UpsertAuthIdentityAsync(userId, authIdentity, cancellationToken);
        }

        public async Task AddAuthIdentity(Guid userId, string identifier, string providerCode, string? secret, CancellationToken ct = default)
        {
            User user = await GetByIdAsync(userId, ct);
            if (user.GetAuthIdentity(providerCode) == null)
                await _userRepository.UpsertAuthIdentityAsync(userId, new AuthIdentity(new AuthProvider(providerCode), identifier, secret), ct);
            else
                throw new AuthIdentityAlreadyExistsException(userId, providerCode);
        }

        public async Task<UserInfo> GetUserInfo(Guid userId, CancellationToken ct)
        {
            User user = await GetByIdAsync(userId, ct);
            if (user.RegistrationState != RegistrationState.Completed)
                throw new UserNotRegistredException(userId);
            IReadOnlyList<ConnectionInfo> connectionInfos = await _connectionService.GetAllUserConnectionsAsync(userId, ct);
            DateTime? lastOnline = DateTime.MinValue;
            foreach (ConnectionInfo connectionInfo in connectionInfos)
            {
                if (connectionInfo.LastOnline == null)
                {
                    lastOnline = null;
                    break;
                }
                if (lastOnline < connectionInfo.LastOnline)
                    lastOnline = connectionInfo.LastOnline;
            }

            return new UserInfo(user.UserData!, lastOnline, user.RegionCode);
        }

        public Task<IReadOnlyList<User>> GetByAuthProviderCode(string providerCode, CancellationToken cancellationToken)
        {
            return _userRepository.GetByAuthProviderCodeAsync(providerCode, cancellationToken);
        }

        public async Task<bool> DeleteAuthIdentityAsync(
            Guid userId,
            string providerCode,
            CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(userId, ct);
            if (user == null)
                throw new UserNotFoundException(userId);

            var authIdentity = user.GetAuthIdentity(providerCode);
            if (authIdentity == null)
                throw new AuthIdentityNotFoundException(userId, providerCode);

            if (user.AuthIdentities.Count <= 1)
                throw new LastAuthIdentityDeletionException(userId);

            return await _userRepository.DeleteAuthIdentityAsync(userId, providerCode, ct);
        }

        public async Task UpsertUserLanguageAsync(
            Guid userId,
            LanguageContext context,
            string languageCode,
            CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(userId, ct);
            if (user == null)
                throw new UserNotFoundException(userId);

            if (string.IsNullOrWhiteSpace(languageCode))
                throw new InvalidLanguageCodeException(languageCode, context.ToString());

            if (languageCode.Length > 5)
                throw new InvalidLanguageCodeException(languageCode, context.ToString());

            if (!Enum.IsDefined(typeof(LanguageContext), context))
                throw new ArgumentException($"Invalid language context: {context}", nameof(context));

            await _userRepository.UpsertUserLanguageAsync(userId, context, languageCode, ct);
        }

        public async Task<string> GetUserLanguageAsync(
            Guid userId,
            LanguageContext context,
            CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(userId, ct);
            if (user == null)
                throw new UserNotFoundException(userId);

            if (user.Language.TryGetValue(context, out var languageCode))
                return languageCode;

            throw new LanguageNotFoundException(userId, context);
        }

        public async Task UpdateUserLanguagesAsync(
            Guid userId,
            Dictionary<LanguageContext, string> languages,
            CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(userId, ct);
            if (user == null)
                throw new UserNotFoundException(userId);

            if (languages == null || languages.Count == 0)
                return;

            foreach (var (context, languageCode) in languages)
            {
                if (string.IsNullOrWhiteSpace(languageCode))
                    throw new InvalidLanguageCodeException(languageCode, context.ToString());

                if (languageCode.Length > 5)
                    throw new InvalidLanguageCodeException(languageCode, context.ToString());

                if (!Enum.IsDefined(typeof(LanguageContext), context))
                    throw new ArgumentException($"Invalid language context: {context}", nameof(context));

                await _userRepository.UpsertUserLanguageAsync(userId, context, languageCode, ct);
            }
        }

        public async Task DeleteUserLanguageAsync(
            Guid userId,
            LanguageContext context,
            CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(userId, ct);
            if (user == null)
                throw new UserNotFoundException(userId);

            if (!user.Language.ContainsKey(context))
                throw new LanguageNotFoundException(userId, context);

            throw new NotImplementedException("Delete language method not implemented");
        }

        public Task<Region> GetRegionByCode(string regionCode, CancellationToken ct = default)
        {
            return _userRepository.GetRegionByCodeAsync(regionCode, ct);
        }
    }
}
