using AIChatWebServer.Hubs.Interfaces;
using AIChatWebServer.Models.Chats;
using AIChatWebServer.Services.Interfaces.Chats;
using AIChatWebServer.Services.Interfaces.Connections;
using ConnectionInfo = AIChatWebServer.Models.Connection.ConnectionInfo;

namespace AIChatWebServer.Hubs.Implementations
{
    public class HubGroupDispatcher(
        IGroupService groupService,
        IChatService chatService,
        IConnectionService connectionService,
        IConnectionStore connectionStore) : IHubGroupDispatcher
    {
        private readonly IGroupService _groupService = groupService;
        private readonly IChatService _chatService = chatService;
        private readonly IConnectionService _connectionService = connectionService;
        private readonly IConnectionStore _connectionStore = connectionStore;

        public async Task AddChatGroup(Guid chatId, CancellationToken ct)
        {
            Chat chat = await _chatService.GetById(chatId, ct);

            var allConnectionIds = new List<string>();

            foreach (var userId in chat.UsersWithData.Keys)
            {
                var connections = await _connectionService.GetAllUserConnectionsAsync(userId, ct);
                var signalRConnections = _connectionStore.GetConnections(connections.Select(c => c.Id));
                allConnectionIds.AddRange(signalRConnections);
            }

            foreach (var connectionId in allConnectionIds)
            {
                await _groupService.AddToChatGroupAsync(connectionId, chatId, ct);
            }
        }

        public async Task AddToChatGroup(Guid userId, Guid chatId, CancellationToken ct)
        {
            var connectionInfos = await _connectionService.GetAllUserConnectionsAsync(userId, ct);
            var connections = _connectionStore.GetConnections(connectionInfos.Select(c => c.Id));

            foreach (var connectionId in connections)
            {
                await _groupService.AddToChatGroupAsync(connectionId, chatId, ct);
            }
        }

        public async Task AddUserGroup(Guid userId, CancellationToken ct)
        {
            var connectionInfos = await _connectionService.GetAllUserConnectionsAsync(userId, ct);
            var connections = _connectionStore.GetConnections(connectionInfos.Select(c => c.Id));

            foreach (var connectionId in connections)
            {
                await _groupService.AddToUserGroupAsync(connectionId, userId, ct);
            }
        }

        public async Task RemoveFromChatGroup(Guid userId, Guid chatId, CancellationToken ct)
        {
            var connectionInfos = await _connectionService.GetAllUserConnectionsAsync(userId, ct);
            var connections = _connectionStore.GetConnections(connectionInfos.Select(c => c.Id));

            foreach (var connectionId in connections)
            {
                await _groupService.RemoveFromChatGroupAsync(connectionId, chatId, ct);
            }
        }

        public async Task RemoveFromUserGroup(Guid userId, CancellationToken ct)
        {
            var connectionInfos = await _connectionService.GetAllUserConnectionsAsync(userId, ct);
            var connections = _connectionStore.GetConnections(connectionInfos.Select(c => c.Id));

            foreach (var connectionId in connections)
            {
                await _groupService.RemoveFromUserGroupAsync(connectionId, userId, ct);
            }
        }

        public async Task DeleteChatGroup(Guid chatId, CancellationToken ct)
        {
            Chat chat = await _chatService.GetById(chatId, ct);

            List<ConnectionInfo> connectionInfos = new List<ConnectionInfo>();

            foreach (var userId in chat.UsersWithData.Keys)
            {
                connectionInfos.AddRange(await _connectionService.GetAllUserConnectionsAsync(userId, ct));
            }

            var connections = _connectionStore.GetConnections(connectionInfos.Select(c => c.Id));

            foreach (var con in connections)
            {
                await _groupService.RemoveFromChatGroupAsync(con, chatId, ct);
            }
        }
    }
}