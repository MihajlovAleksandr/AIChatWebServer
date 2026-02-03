using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Tokens.Consts;

namespace AIChatWebServer.Services.Context.Implementations
{
    public class EntryTokenContext(IHttpContextAccessor httpContextAccessor) : BaseTokenContext(httpContextAccessor), IEntryTokenContext
    {
        public override JwtTokenType TokenType => JwtTokenType.Entry;

        public string? Code => TryGetClaimValue("code");
    }
}
