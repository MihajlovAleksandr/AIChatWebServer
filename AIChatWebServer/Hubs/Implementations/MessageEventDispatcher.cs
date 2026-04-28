using AIChatWebServer.DTO.Response;
using AIChatWebServer.Hubs.Interfaces;

namespace AIChatWebServer.Hubs.Implementations
{
    public class MessageEventDispatcher(IMessageDispatcher messageDispatcher) : IMessageEventDispatcher
    {
        private readonly IMessageDispatcher _messageDispatcher = messageDispatcher;

        public async Task MessageSent(Guid userId, Guid? excludedConnectionId, MessageResponse response, CancellationToken ct)
        {
            Guid[] ids = excludedConnectionId == null ? [] : [excludedConnectionId.Value]; 
            await _messageDispatcher.SendToUserAsync(userId, ids, nameof(MessageSent), response, ct);
        }

        public async Task MessageEdited(Guid chatId, Guid excludedConnectionId, MessageUpdatedResponse response, CancellationToken ct)
        { 
            await _messageDispatcher.SendToChatAsync(chatId, [excludedConnectionId], nameof(MessageEdited), response, ct);
        }

        public async Task MessageDeleted(Guid chatId, Guid excludedConnectionId, MessageDeletedResponse response, CancellationToken ct)
        {
            await _messageDispatcher.SendToChatAsync(chatId, [excludedConnectionId], nameof(MessageDeleted), response, ct);
        }

        public async Task MessageFileDeleted(Guid chatId, Guid excludedConnectionId, MessageFileDeletedResponse response, CancellationToken ct)
        {
            await _messageDispatcher.SendToChatAsync(chatId, [excludedConnectionId], nameof(MessageFileDeleted), response, ct);
        }

        public async Task MessageStatusUpdated(Guid userId, Guid? excludedConnectionId, MessageStatusUpdatedResponse response, CancellationToken ct)
        {
            Guid[] ids = excludedConnectionId == null ? [] : [excludedConnectionId.Value];
            await _messageDispatcher.SendToUserAsync(userId, ids, nameof(MessageStatusUpdated), response, ct);
        }

    }
}
