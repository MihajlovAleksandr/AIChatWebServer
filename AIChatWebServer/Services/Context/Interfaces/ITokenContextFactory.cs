using AIChatWebServer.Services.Context.Interfaces;

namespace AIChatWebServer.Services.Tokens.Interfaces
{
    public interface ITokenContextFactory
    {
        ITokenContext Create();
    }
}
