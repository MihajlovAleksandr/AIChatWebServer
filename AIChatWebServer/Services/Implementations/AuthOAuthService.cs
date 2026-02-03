using AIChatWebServer.Models.Exceptions;
using AIChatWebServer.Models.User;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces;
using Npgsql;

namespace AIChatWebServer.Services.Implementations
{
    public sealed class AuthOAuthService(
        IUserRepository repository,
        IHasher hasher) : IAuthOAuthService
    {
        private readonly IUserRepository _repository = repository;
        private readonly IHasher _hasher = hasher;

        private const string GoogleProvider = "GOOGLE";

        public async Task<User?> LoginGoogleAsync(
            string email,
            string googleId,
            CancellationToken ct = default)
        {
            User? user =
                await _repository.GetByAuthIdentityCodeAsync(
                    GoogleProvider,
                    email,
                    ct);

            if (user == null)
                return null;

            var identity =
                user.GetAuthIdentity(GoogleProvider);

            if (identity?.Secret == null)
                return null;

            if (!_hasher.Verify(googleId, identity.Secret))
                return null;

            var ban =
                await _repository.GetUserBanByIdAsync(
                    user.Id,
                    ct);

            if (ban != null && ban.IsActual())
                throw new UserBannedException(ban);

            return user;
        }

        public async Task<Guid> RegisterGoogleAsync(
            string email,
            string googleId,
            string regionCode,
            string languageCode,
            CancellationToken ct = default)
        {
            try
            {
                Guid userId =
                    await _repository.CreateUserAsync(
                        email,
                        regionCode,
                        languageCode,
                        new AuthIdentity(
                            new AuthProvider(GoogleProvider),
                            email,
                            _hasher.Hash(googleId)),
                        ct);

                await _repository.MarkEmailVerifiedAsync(userId, ct);

                return userId;
            }
            catch (PostgresException ex)
                when (ex.SqlState == "23505")
            {
                throw new UserAlreadyExistsException(email);
            }
        }
    }
}
