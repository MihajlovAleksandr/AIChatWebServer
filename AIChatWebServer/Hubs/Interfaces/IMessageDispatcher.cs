namespace AIChatWebServer.Hubs.Interfaces
{
    public interface IMessageDispatcher
    {
        Task SendToUserAsync<T>(Guid userId, IReadOnlyList<Guid> excluded, string command, T payload, CancellationToken ct);
        Task SendToChatAsync<T>(Guid chatId, IReadOnlyList<Guid> excluded, string command, T payload, CancellationToken ct);
        Task SendToConnectionAsync<T>(Guid connectionId, string command, T payload, CancellationToken ct);
        Task SendToUserAsync(Guid userId, IReadOnlyList<Guid> excluded, string command, CancellationToken ct);
        Task SendToChatAsync(Guid chatId, IReadOnlyList<Guid> excluded, string command, CancellationToken ct);
        Task SendToConnectionAsync(Guid connectionId, string command, CancellationToken ct);
    }
}
