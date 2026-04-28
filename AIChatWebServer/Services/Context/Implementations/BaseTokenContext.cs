using AIChatWebServer.Models.Exceptions.Implementations.Auth;
using AIChatWebServer.Services.Context.Consts;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Utils.Errors;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AIChatWebServer.Services.Context.Implementations
{
    public abstract class BaseTokenContext(IUserContextAccessor userContextAccessor) : ITokenContext
    {
        protected readonly IUserContextAccessor _userContextAccessor = userContextAccessor
            ?? throw new ArgumentNullException(nameof(userContextAccessor));

        public Guid UserId =>
            Guid.TryParse(TryGetClaimValue(ClaimTypes.NameIdentifier), out var guid)
                ? guid
                : throw new AuthTokenException(SessionErrors.InvalidToken);

        public DateTime ExpiresAtUtc
        {
            get
            {
                var value = TryGetClaimValue(JwtRegisteredClaimNames.Exp);

                if (!long.TryParse(value, out var seconds))
                    throw new AuthTokenException(SessionErrors.InvalidToken);

                return DateTimeOffset
                    .FromUnixTimeSeconds(seconds)
                    .UtcDateTime;
            }
        }

        public abstract JwtTokenType TokenType { get; }

        protected string TryGetClaimValue(string claimType)
        {
            return _userContextAccessor.User?
                .FindFirst(claimType)?
                .Value ?? throw new AuthTokenException(SessionErrors.InvalidToken);
        }
    }
}