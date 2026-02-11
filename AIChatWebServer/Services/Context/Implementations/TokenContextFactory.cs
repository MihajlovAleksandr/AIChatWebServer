using AIChatWebServer.Models.Exceptions.Implementations.Auth;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Tokens.Consts;
using AIChatWebServer.Services.Tokens.Interfaces;
using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Services.Context.Implementations
{
    public sealed class TokenContextFactory(
        IHttpContextAccessor accessor) : ITokenContextFactory
    {
        private readonly IHttpContextAccessor _accessor = accessor;


        public ITokenContext Create()
        {
            var user = _accessor.HttpContext?.User;

            if (user == null || user.Identity?.IsAuthenticated != true)
            {
                throw new AuthTokenException(
                    TokenErrors.Unauthorized);
            }

            string? typeValue =
                user.FindFirst(JwtClaimNames.TokenType)?.Value;


            if (string.IsNullOrWhiteSpace(typeValue))
            {
                throw new AuthTokenException(
                    TokenErrors.InvalidToken);
            }


            if (!Enum.TryParse<JwtTokenType>(typeValue, out var tokenType))
            {
                throw new AuthTokenException(
                    TokenErrors.InvalidType);
            }

            return tokenType switch
            {
                JwtTokenType.Work =>
                    new WorkTokenContext(_accessor),

                JwtTokenType.Registration =>
                    new RegistrationTokenContext(_accessor),

                JwtTokenType.Entry =>
                    new EntryTokenContext(_accessor),

                _ =>
                    throw new AuthTokenException(
                        TokenErrors.InvalidType)
            };
        }
    }
}
