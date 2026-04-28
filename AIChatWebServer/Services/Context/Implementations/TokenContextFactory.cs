using AIChatWebServer.Models.Exceptions.Implementations.Auth;
using AIChatWebServer.Services.Context.Consts;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Services.Context.Implementations
{
    public sealed class TokenContextFactory : ITokenContextFactory
    {
        public ITokenContext Create(IUserContextAccessor accessor)
        {
            var user = accessor.User;

            if (user == null || user.Identity?.IsAuthenticated != true)
            {
                throw new AuthTokenException(TokenErrors.Unauthorized);
            }

            string? typeValue =
                user.FindFirst(JwtClaimNames.TokenType)?.Value;

            if (string.IsNullOrWhiteSpace(typeValue))
            {
                throw new AuthTokenException(TokenErrors.InvalidToken);
            }

            if (!Enum.TryParse<JwtTokenType>(typeValue, out var tokenType))
            {
                throw new AuthTokenException(TokenErrors.InvalidType);
            }

            return tokenType switch
            {
                JwtTokenType.Work => new WorkTokenContext(accessor),
                JwtTokenType.Registration => new RegistrationTokenContext(accessor),
                JwtTokenType.Entry => new EntryTokenContext(accessor),
                _ => throw new AuthTokenException(TokenErrors.InvalidType)
            };
        }
    }
}