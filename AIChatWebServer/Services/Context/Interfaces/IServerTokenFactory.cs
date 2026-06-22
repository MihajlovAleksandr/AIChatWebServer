using AIChatWebServer.Services.Context.Consts;

namespace AIChatWebServer.Services.Context.Interfaces
{
    public interface IServerTokenFactory
    {
        string Create(Guid userId, Servers server);
    }
}
