using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Tokens.Consts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AIChatWebServer.Services.Context.Implementations
{
    public abstract class BaseTokenContext(IHttpContextAccessor httpContextAccessor) : ITokenContext
    {

        protected readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor
                ?? throw new ArgumentNullException(nameof(httpContextAccessor));

        public Guid? UserId =>
            Guid.TryParse(TryGetClaimValue(ClaimTypes.NameIdentifier), out var guid)
                ? guid
                : null;
        public DateTime? ExpiresAtUtc
        {
            get
            {
                var value = TryGetClaimValue(JwtRegisteredClaimNames.Exp);

                if (!long.TryParse(value, out var seconds))
                    return null;

                return DateTimeOffset
                    .FromUnixTimeSeconds(seconds)
                    .UtcDateTime;
            }
        }
        public abstract JwtTokenType TokenType { get; }

        protected string? TryGetClaimValue(string claimType)
        {
            return _httpContextAccessor.HttpContext?
                .User?
                .FindFirst(claimType)?
                .Value;
        }
    }
}
