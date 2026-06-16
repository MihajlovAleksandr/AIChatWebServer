using AIChatWebServer.DTO.Response;

namespace AIChatWebServer.Hubs.Interfaces
{
    public interface IConnectionEventDispatcher
    {
        Task ConnectionChanged(Guid userId, Guid[] excludedConnectionIds, ConnectionChangedResponse response);
        Task OnlineStatusChanged(Guid chatId, Guid excludedConnectionId, OnlineStatusChangedResponse response);
        Task SyncDB(Guid connectionId, SyncResponse response);
        Task Logout(Guid connectionId);
        Task EntryCodeUsed(Guid connectionId);
    }
}
