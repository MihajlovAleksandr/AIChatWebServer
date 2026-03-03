using AIChatWebServer.Models.Chats;

namespace AIChatWebServer.Services.Interfaces.Chats
{
    public interface IChatService
    {
        Task<Guid> CreateAsync(ChatType type, IEnumerable<Guid> userIds, string name, CancellationToken cancellationToken = default);
        Task<Chat> GetById(Guid id, CancellationToken cancellationToken = default);
        Task ExecuteAction(Guid chatId, ChatAction action, CancellationToken cancellationToken = default);
    }
}
