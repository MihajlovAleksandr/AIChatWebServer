using AIChatWebServer.Models.Chats.Matchmaking;

namespace AIChatWebServer.Services.Interfaces.Chats.Matchmaking
{
    public interface IChatAddUserStrategy : IChatMatchStrategy
    {
        Task<ChatMatchmakingResult?> MatchChatAsync(
            Guid userId, 
            Guid chatId, 
            int slot, 
            string userPredicate, 
            CancellationToken ct);
    }
}
