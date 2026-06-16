using AIChatWebServer.Models.Chats;

namespace AIChatWebServer.Hubs.Interfaces
{
    public interface IChatGroupNotifier
    {
        Task ChatCreated(Guid chatId, Guid userId, Guid? excludedConnectionId, CancellationToken ct);
        Task ChatCreated(Guid chatId, Guid? excludedConnectionId, CancellationToken ct);
        Task ChatNameUpdated(Guid chatId, Guid userId, Guid excludedConnectionId, string name, CancellationToken ct);
        Task ChatEnded(Chat chat, Guid excludedConnectionId, CancellationToken ct);
        Task UserRemoved(Guid chatId, Guid removedUserId, Guid excludedConnectionId, CancellationToken ct);
        Task UserAdded(Guid chatId, Guid userId, CancellationToken ct);

        Task ChatSearchingStatusUpdated(Guid userId, Guid excludedConnectionId, bool isSearching);
        Task GroupSearchingStatusUpdated(Guid userId, Guid excludedConnectionId, bool isSearching, Guid? chatId);
        Task Typing(Guid chatId, Guid userId, bool isTyping);
    }
}