using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Chats.Matchmaking;

namespace AIChatWebServer.Services.Interfaces.Chats.Matchmaking
{
    public interface IChatMatchStrategy
    {
        ChatType MatchType { get; }

        Task<ChatMatchmakingResult?> MatchUserAsync(
            Guid userId,
            string userPredicate,
            string chatName,
            CancellationToken ct);
    }
}
