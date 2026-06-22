using AIChatWebServer.Hubs.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace AIChatWebServer.Hubs.Implementations
{
    public class MessageDispatcher(
        IHubContext<ChatHub> hubContext,
        IGroupService groupService,
        IConnectionStore connectionStore) : IMessageDispatcher
    {
        private readonly IHubContext<ChatHub> _hubContext = hubContext;
        private readonly IGroupService _groupService = groupService;
        private readonly IConnectionStore _store = connectionStore;

        public Task SendToUserAsync<T>(
            Guid userId,
            IReadOnlyList<Guid> excluded,
            string command,
            T payload,
            CancellationToken ct)
        {
            var group = _groupService.GetUserGroup(userId);

            return _hubContext.Clients
                .GroupExcept(group, _store.GetConnections(excluded))
                .SendAsync(command, payload, ct);
        }

        public Task SendToChatAsync<T>(
            Guid chatId,
            IReadOnlyList<Guid> excluded,
            string command,
            T payload,
            CancellationToken ct)
        {
            var group = _groupService.GetChatGroup(chatId);

            return _hubContext.Clients
                .GroupExcept(group, _store.GetConnections(excluded))
                .SendAsync(command, payload, ct);
        }

        public Task SendToConnectionAsync<T>(
            Guid connectionId,
            string command,
            T payload,
            CancellationToken ct)
        {
            IReadOnlyList<string> connections = _store.GetConnections(connectionId).ToList();

            return _hubContext.Clients
                .Clients(connections)
                .SendAsync(command, payload, ct);
        }

        public Task SendToUserAsync(
            Guid userId,
            IReadOnlyList<Guid> excluded,
            string command,
            CancellationToken ct)
        {
            var group = _groupService.GetUserGroup(userId);

            return _hubContext.Clients
                .GroupExcept(group, _store.GetConnections(excluded))
                .SendAsync(command, ct);
        }

        public Task SendToChatAsync(
            Guid chatId,
            IReadOnlyList<Guid> excluded,
            string command,
            CancellationToken ct)
        {
            var group = _groupService.GetChatGroup(chatId);

            return _hubContext.Clients
                .GroupExcept(group, _store.GetConnections(excluded))
                .SendAsync(command, ct);
        }

        public Task SendToConnectionAsync(
            Guid connectionId,
            string command,
            CancellationToken ct)
        {
            IReadOnlyList<string> connections = _store.GetConnections(connectionId).ToList();

            return _hubContext.Clients
                .Clients(connections)
                .SendAsync(command, ct);
        }
    }
}
