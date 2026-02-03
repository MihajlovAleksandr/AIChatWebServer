using AIChatWebServer.Services.Context.Interfaces;

namespace AIChatWebServer.Services.Tokens.Interfaces
{
    public interface IWorkTokenContext : ITokenContext
    {
        Guid? ConnectionId { get; }
    }
}
