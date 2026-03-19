using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Chats.ValidateSettings;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IChatRepository
    {
        Task<Guid> CreateAsync(ChatType type, IDictionary<Guid, string> creatorsWithChatNames, CancellationToken cancellationToken = default);
        Task<Chat?> GetById(Guid id, CancellationToken cancellationToken = default);
        Task UpdateName(Guid id, Guid userId, string name, CancellationToken cancellationToken = default);
        Task AddUser(Guid id, Guid userId, string name, ChatUserRole role, CancellationToken cancellationToken = default);
        Task RemoveUser(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task End(Guid id, DateTime endTime, CancellationToken cancellationToken = default);
        Task UpdateChatSettings(Guid chatId, ChatSettings settings, CancellationToken cancellationToken = default);
        Task UpdateUserSettings(Guid chatId, Guid userId, UserSettings settings, CancellationToken cancellationToken = default);
    }
}