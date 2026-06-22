namespace AIChatWebServer.Hubs.Interfaces
{
    public interface IHubGroupDispatcher
    {
        Task AddChatGroup(Guid chatId, CancellationToken ct);
        Task AddToChatGroup(Guid userId, Guid chatId, CancellationToken ct);
        Task AddUserGroup(Guid userId, CancellationToken ct);
        Task RemoveFromChatGroup(Guid userId, Guid chatId, CancellationToken ct);
        Task RemoveFromUserGroup(Guid userId, CancellationToken ct);
        Task DeleteChatGroup(Guid chatId, CancellationToken ct);
    }
}