using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Sync;

namespace AIChatWebServer.Services.Interfaces.Chats
{
    public interface IChatService
    {
        Task<Guid> CreateAsync(ChatType type, IDictionary<Guid, string> creatorsWithChatNames, CancellationToken cancellationToken = default);
        Task<Chat> GetById(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Chat>> GetByUserId(Guid userId, CancellationToken cancellationToken = default);
        Task<Guid> GetUserIdByChatUserId(Guid chatUserId, CancellationToken ct = default);
        Task ExecuteAction(Guid chatId, ChatAction action, CancellationToken cancellationToken = default);
        Task<SyncChats> SyncAsync(Guid userId, DateTime since, CancellationToken ct);
    }
}
