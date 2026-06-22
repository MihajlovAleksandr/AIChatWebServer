using AIChatWebServer.Hubs.Interfaces;
using AIChatWebServer.Models.Chats;
using AIChatWebServer.Services.Interfaces.Chats;

namespace AIChatWebServer.Hubs.Implementations
{
    public class DisconnectService(
        IConnectionStore connectionStore,
        IGroupService groupService,
        IChatService chatService,
        IConnectionNotifier connectionNotifier) : IDisconnectService
    {
        private readonly IConnectionStore _connectionStore = connectionStore;
        private readonly IGroupService _groupService = groupService;
        private readonly IChatService _chatService = chatService;
        private readonly IConnectionNotifier _connectionNotifier = connectionNotifier;

        public async Task DisconnectAync(Guid connectionId, Guid userId, Guid initiatorConnectionId, CancellationToken ct)
        {
            await _connectionNotifier.Logout(connectionId, userId, initiatorConnectionId);
            IReadOnlyList<string>? connections = _connectionStore.RemoveAll(connectionId);
            IReadOnlyList<Chat> chats = await _chatService.GetByUserId(connectionId);
            if (connections == null) return;
            foreach (var connection in connections)
            {
                foreach (var chat in chats)
                    await _groupService.RemoveFromChatGroupAsync(connection, chat.Id, ct);
                await _groupService.RemoveFromUserGroupAsync(connection, userId, ct);
            }
        }
    }
}
