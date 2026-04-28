using AIChatWebServer.DTO.Response;
using AIChatWebServer.Hubs.Interfaces;

namespace AIChatWebServer.Hubs.Implementations
{
    public class ChatEventsDispatcher(IMessageDispatcher messageDispatcher) : IChatEventsDispatcher
    {
        private readonly IMessageDispatcher _messageDispatcher = messageDispatcher;

        public async Task ChatCreated(Guid userId, Guid? excludedConnectionId, ChatResponse response)
        {
            Guid[] excluded = excludedConnectionId != null ? [excludedConnectionId.Value ] : [];
            await _messageDispatcher.SendToUserAsync(userId, excluded, nameof(ChatCreated), response, CancellationToken.None);
        }

        public async Task ChatDeleted(Guid userId, Guid excludedConnectionId, ChatDeletedResponse response)
        {
            await _messageDispatcher.SendToUserAsync(userId, [excludedConnectionId], nameof(ChatDeleted), response, CancellationToken.None);
        }

        public async Task ChatEnded(Guid chatId, Guid excludedConnectionId, ChatEndedResponse response)
        {
            await _messageDispatcher.SendToChatAsync(chatId, [excludedConnectionId], nameof(ChatEnded), response, CancellationToken.None);
        }

        public async Task ChatNameUpdated(Guid chatId, Guid excludedConnectionId, ChatNameUpdatedResponse response)
        {
            await _messageDispatcher.SendToChatAsync(chatId, [excludedConnectionId], nameof(ChatNameUpdated), response, CancellationToken.None);
        }

        public async Task ChatUserAdded(Guid chatId, Guid? excludedConnectionId, ChatUserActionResponse response)
        {
            Guid[] excluded = excludedConnectionId != null ? [excludedConnectionId.Value] : [];
            await _messageDispatcher.SendToChatAsync(chatId, excluded, nameof(ChatUserAdded), response, CancellationToken.None);
        }

        public async Task ChatUserRemoved(Guid chatId, Guid excludedConnectionId, ChatUserActionResponse response)
        {
            await _messageDispatcher.SendToChatAsync(chatId, [excludedConnectionId], nameof(ChatUserRemoved), response, CancellationToken.None);
        }

        public async Task ChatSearchingStatusUpdated(Guid userId, Guid excludedConnectionId, ChatSeachingStatusResponse response)
        {
            await _messageDispatcher.SendToUserAsync(userId, [excludedConnectionId], nameof(ChatSearchingStatusUpdated), response, CancellationToken.None);
        }

        public async Task GroupSearchingStatusUpdated(Guid userId, Guid excludedConnectionId, GroupSeachingStatusResponse response)
        {
            await _messageDispatcher.SendToUserAsync(userId, [excludedConnectionId], nameof(GroupSearchingStatusUpdated), response, CancellationToken.None);
        }
    }
}
