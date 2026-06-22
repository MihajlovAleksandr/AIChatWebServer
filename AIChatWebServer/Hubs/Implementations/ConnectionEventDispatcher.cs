using AIChatWebServer.DTO.Response;
using AIChatWebServer.Hubs.Interfaces;

namespace AIChatWebServer.Hubs.Implementations
{
    public class ConnectionEventDispatcher(IMessageDispatcher messageDispatcher) : IConnectionEventDispatcher
    {
        private readonly IMessageDispatcher _messageDispatcher = messageDispatcher;

        public async Task ConnectionChanged(Guid userId, Guid[] excludedConnectionIds, ConnectionChangedResponse response)
        {
            await _messageDispatcher.SendToUserAsync(userId, excludedConnectionIds, nameof(ConnectionChanged), response, CancellationToken.None);
        }

        public async Task OnlineStatusChanged(Guid chatId, Guid excludedConnectionId, OnlineStatusChangedResponse response)
        {
            await _messageDispatcher.SendToChatAsync(chatId, [excludedConnectionId], nameof(OnlineStatusChanged), response, CancellationToken.None);
        }

        public async Task Logout(Guid connectionId)
        {
            await _messageDispatcher.SendToConnectionAsync(connectionId, nameof(Logout), CancellationToken.None);
        }

        public async Task SyncDB(Guid connectionId, SyncResponse response)
        {
            await _messageDispatcher.SendToConnectionAsync(connectionId, nameof(SyncDB), response, CancellationToken.None);
        }

        public async Task EntryCodeUsed(Guid connectionId)
        {
            await _messageDispatcher.SendToConnectionAsync(connectionId, nameof(EntryCodeUsed), "ssds", CancellationToken.None);
        }
    }
}
