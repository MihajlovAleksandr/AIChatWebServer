using AIChatWebServer.Models.Chats;

namespace AIChatWebServer.Services.Interfaces.Chats
{
    public interface IChatService
    {
        Task<Guid> CreateAsync(ChatType type, IDictionary<Guid, string> creatorsWithChatNames, CancellationToken cancellationToken = default);
        Task<Chat> GetById(Guid id, CancellationToken cancellationToken = default);
        Task ExecuteAction(Guid chatId, ChatAction action, CancellationToken cancellationToken = default);
    }
}
