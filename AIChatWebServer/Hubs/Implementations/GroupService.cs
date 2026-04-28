using AIChatWebServer.Hubs.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace AIChatWebServer.Hubs.Implementations
{
    public class GroupService(IHubContext<ChatHub> hubContext) : IGroupService
    {
        private readonly IHubContext<ChatHub> _hubContext = hubContext;

        public string GetUserGroup(Guid userId) => $"user_{userId}";
        public string GetChatGroup(Guid chatId) => $"chat_{chatId}";

        public Task AddToUserGroupAsync(string connectionId, Guid userId, CancellationToken ct)
        {
            return _hubContext.Groups.AddToGroupAsync(connectionId, GetUserGroup(userId), ct);
        }

        public Task AddToChatGroupAsync(string connectionId, Guid chatId, CancellationToken ct)
        {
            return _hubContext.Groups.AddToGroupAsync(connectionId, GetChatGroup(chatId), ct);
        }

        public Task RemoveFromChatGroupAsync(string connectionId, Guid chatId, CancellationToken ct)
        {
            return _hubContext.Groups.RemoveFromGroupAsync(connectionId, GetChatGroup(chatId), ct);
        }

        public Task RemoveFromUserGroupAsync(string connectionId, Guid userId, CancellationToken ct)
        {
            return _hubContext.Groups.RemoveFromGroupAsync(connectionId, GetUserGroup(userId), ct);
        }


    }
}
