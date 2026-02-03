using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Context.Implementations;
using AIChatWebServer.Services.Tokens.Consts;
using AIChatWebServer.Services.Tokens.Interfaces;

namespace AIChatWebServer.Services.Tokens.Implementations
{
    public class RegistrationTokenContext(IHttpContextAccessor accessor)
                : BaseTokenContext(accessor), IRegistrationTokenContext
    {
        public override JwtTokenType TokenType => JwtTokenType.Registration;

        public RegistrationState? RegistrationState => 
            Enum.TryParse(typeof(RegistrationState), 
                TryGetClaimValue("registrationState"), out var result) 
            ? (RegistrationState)result : null;

        public Guid? ConnectionId => 
            Guid.TryParse(TryGetClaimValue("connectionId"), out var result) 
            ? result : null;
    }
}
