using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Chats.Matchmaking;

namespace AIChatWebServer.Services.Interfaces.Chats.Matchmaking
{
    public interface IDirectMatchmakingService
    {
        Task<ChatMatchmakingResult?> MatchUserAsync(
            ChatType chatType,
            Guid userId,
            string userPredicate,
            string chatName,
            CancellationToken ct);

        Task<bool> IsSearching(Guid userId,
            CancellationToken ct);

        Task CancelSearch(Guid userId, 
            CancellationToken ct);
    }
}
