using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Chats.Matchmaking;

namespace AIChatWebServer.Services.Interfaces.Chats.Matchmaking
{
    public interface IChatMatchStrategiesHandler
    {
        Task<ChatMatchmakingResult?> MatchUserAsync(
            ChatType chatType,
            Guid userId,
            string userPredicate,
            string chatName,
            CancellationToken ct);
    }
}
