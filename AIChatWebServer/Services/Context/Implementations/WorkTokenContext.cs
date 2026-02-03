using AIChatWebServer.Services.Context.Implementations;
using AIChatWebServer.Services.Tokens.Consts;
using AIChatWebServer.Services.Tokens.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace AIChatWebServer.Security.Contexts
{
    internal sealed class WorkTokenContext
        : BaseTokenContext, IWorkTokenContext
    {
        public WorkTokenContext(IHttpContextAccessor accessor)
            : base(accessor)
        {
        }

        public Guid? ConnectionId =>
            Guid.TryParse(TryGetClaimValue("connectionId"), out var guid)
                ? guid
                : null;

        public bool IsExpired =>
            ExpiresAtUtc.HasValue &&
            ExpiresAtUtc.Value <= DateTime.UtcNow;


        public override JwtTokenType TokenType =>
            JwtTokenType.Work;
    }
}
