using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Chats.Matchmaking;
using AIChatWebServer.Models.Sync;

namespace AIChatWebServer.Services.Interfaces.Chats.Matchmaking
{
    public interface IGroupMatchmakingService
    {
        Task<ChatMatchmakingResult?> MatchUserAsync(
            Guid userId,
            string userPredicate,
            string chatName,
            CancellationToken ct = default);

        Task<ChatMatchmakingResult?> MatchChatAsync(
            Guid chatId, 
            StartSearchChatAction action, 
            CancellationToken ct = default);

        Task<bool> IsSearching(Guid userId,
            CancellationToken ct);

        Task<SyncGroupMatchmaking> SyncAsync(Guid userId,
            CancellationToken ct = default);

        Task CancelSearch(Guid userId,
            CancellationToken ct);
    }
}
