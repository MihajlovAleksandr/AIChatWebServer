using System.Collections;

namespace AIChatWebServer.Hubs.Interfaces
{
    public interface IDisconnectService
    {
        Task DisconnectAync(Guid connectionId, Guid userId, Guid initiatorConnectionId, CancellationToken ct);
    }
}
