namespace AIChatWebServer.Hubs.Interfaces
{
    public interface IGroupService
    {
        string GetUserGroup(Guid userId);
        string GetChatGroup(Guid chatId);

        Task AddToUserGroupAsync(string connectionId, Guid userId, CancellationToken ct);
        Task AddToChatGroupAsync(string connectionId, Guid chatId, CancellationToken ct);
        Task RemoveFromChatGroupAsync(string connectionId, Guid chatId, CancellationToken ct);
        Task RemoveFromUserGroupAsync(string connectionId, Guid userId, CancellationToken ct);

    }
}
