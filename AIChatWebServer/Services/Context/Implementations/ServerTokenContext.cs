using AIChatWebServer.Models.Exceptions.Implementations.Auth;
using AIChatWebServer.Services.Context.Consts;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Services.Context.Implementations
{
    internal sealed class ServerTokenContext(IUserContextAccessor accessor)
                  : BaseTokenContext(accessor), IServerTokenContext
    {
        public Servers Server =>
            Servers.TryParse(TryGetClaimValue("server"), out Servers result)
                ? result
                : throw new AuthTokenException(SessionErrors.InvalidToken);

        public override JwtTokenType TokenType =>
            JwtTokenType.Server;
    }
}
