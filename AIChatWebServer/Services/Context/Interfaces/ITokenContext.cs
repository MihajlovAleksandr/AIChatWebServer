using AIChatWebServer.Services.Tokens.Consts;

namespace AIChatWebServer.Services.Context.Interfaces
{
    public interface ITokenContext
    {
        Guid UserId { get; }
        DateTime ExpiresAtUtc { get; }
        JwtTokenType TokenType { get; }
    }
}
