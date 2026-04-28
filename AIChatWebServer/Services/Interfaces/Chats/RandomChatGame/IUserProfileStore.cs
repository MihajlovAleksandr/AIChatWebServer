using AIChatWebServer.Models.Chats.RandomChat;

namespace AIChatWebServer.Services.Interfaces.Chats.RandomChatGame
{
    public interface IUserProfileStore
    {
        Task CreateAsync(
            Guid chatId,
            UserProfile profile,
            TimeSpan? ttl = null,
            CancellationToken ct = default);

        Task<UserProfile?> GetAsync(
            Guid chatId,
            CancellationToken ct = default);

        Task<bool> DeleteAsync(
            Guid chatId,
            CancellationToken ct = default);
    }
}