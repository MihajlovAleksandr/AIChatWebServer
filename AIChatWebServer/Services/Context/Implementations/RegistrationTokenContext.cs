using AIChatWebServer.Models.Exceptions.Implementations.Auth;
using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Context.Consts;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Services.Context.Implementations
{
    public sealed class RegistrationTokenContext(IHttpContextAccessor accessor)
                : BaseTokenContext(accessor), IRegistrationTokenContext
    {
        public override JwtTokenType TokenType => JwtTokenType.Registration;

        public RegistrationState RegistrationState => 
            Enum.TryParse(typeof(RegistrationState), 
                TryGetClaimValue("registrationState"), out var result) 
            ? (RegistrationState)result : throw new AuthTokenException(SessionErrors.InvalidToken);

        public Guid ConnectionId => 
            Guid.TryParse(TryGetClaimValue("connectionId"), out var result) 
            ? result : throw new AuthTokenException(SessionErrors.InvalidToken);
    }
}
