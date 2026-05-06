using AIChatWebServer.Services.Context.Consts;

namespace AIChatWebServer.Services.Context.Interfaces
{
    public interface IServerTokenContext : ITokenContext
    {
        Servers Server { get; }
    }
}
