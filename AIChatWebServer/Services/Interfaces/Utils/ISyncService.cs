using AIChatWebServer.Models.Sync;

namespace AIChatWebServer.Services.Interfaces.Utils
{
    public interface ISyncService
    {
        Task<SyncModel> SyncAsync(Guid userId, DateTime? lastOnline, CancellationToken ct = default);
    }
}
