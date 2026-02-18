using AIChatWebServer.Services.Context.Consts;
using AIChatWebServer.Services.Context.Interfaces;

namespace AIChatWebServer.Services.Context.Implementations
{
    public sealed class EntryTokenContext(IHttpContextAccessor httpContextAccessor) : BaseTokenContext(httpContextAccessor), IEntryTokenContext
    {
        public override JwtTokenType TokenType => JwtTokenType.Entry;

        public string Code => TryGetClaimValue("code");
    }
}
