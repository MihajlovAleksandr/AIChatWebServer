using AIChatWebServer.Models.Chats.RandomChat;
using AIChatWebServer.Models.User;

namespace AIChatWebServer.Services.Interfaces.Chats.RandomChatGame
{
    public interface IUserProfileGenerator
    {
        Task<UserProfile> GenerateAync(Guid chatId, User user, CancellationToken ct);
    }
}
