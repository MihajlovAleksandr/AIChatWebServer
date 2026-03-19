using AIChatWebServer.Models.Exceptions.Implementations.Auth;
using AIChatWebServer.Models.Exceptions.Implementations.Auth.Login;
using AIChatWebServer.Models.Exceptions.Implementations.User;
using AIChatWebServer.Models.User;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces;

namespace AIChatWebServer.Services.Implementations
{
    public sealed class AuthLoginService(
        IUserRepository repository,
        IHasher hasher) : IAuthLoginService
    {
        private readonly IUserRepository _repository = repository;
        private readonly IHasher _hasher = hasher;

        public async Task<User> LoginAsync(
            string identifier,
            string secret,
            string providerCode,
            CancellationToken ct = default)
        {
            User user =
                await _repository.GetByAuthIdentityCodeAsync(
                    providerCode,
                    identifier,
                    ct) ?? throw new UserNotFoundException(identifier);

            var identity =
                user.GetAuthIdentity(providerCode);

            if (identity?.Secret == null)
                throw new InvalidLoginProviderException(identifier, providerCode);

            if (!_hasher.Verify(secret, identity.Secret))
                throw new InvalidCredentialsException(user.Id);

            var ban =
                await _repository.GetUserBanByIdAsync(
                    user.Id,
                    ct);

            if (ban != null && ban.IsActual())
                throw new UserBannedException(ban);

            return user;
        }
    }
}
