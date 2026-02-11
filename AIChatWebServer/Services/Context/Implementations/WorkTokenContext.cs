using AIChatWebServer.Models.Exceptions.Implementations.Auth;
using AIChatWebServer.Services.Tokens.Consts;
using AIChatWebServer.Services.Tokens.Interfaces;
using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Services.Context.Implementations
{
    internal sealed class WorkTokenContext
        : BaseTokenContext, IWorkTokenContext
    {
        public WorkTokenContext(IHttpContextAccessor accessor)
            : base(accessor)
        {
        }

        public Guid ConnectionId =>
            Guid.TryParse(TryGetClaimValue("connectionId"), out var guid)
                ? guid
                : throw new AuthTokenException(SessionErrors.InvalidToken);

        public override JwtTokenType TokenType =>
            JwtTokenType.Work;
    }
}
