using AIChatWebServer.Services.Context.Consts;

namespace AIChatWebServer.Services.Interfaces
{
    public interface IServerValidator
    {
        Task Validate(Guid userId, Servers server, CancellationToken ct);
    }
}
