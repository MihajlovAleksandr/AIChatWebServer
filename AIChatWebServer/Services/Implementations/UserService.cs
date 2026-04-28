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
    }
}
