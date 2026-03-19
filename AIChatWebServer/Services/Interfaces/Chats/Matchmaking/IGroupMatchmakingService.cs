using AIChatWebServer.Models.Chats;

namespace AIChatWebServer.Services.Interfaces.Chats.Matchmaking
{
    public interface IGroupMatchmakingService
    {
        Task MatchUserAsync(
            Guid userId,
            string userPredicate,
            string chatName,
            CancellationToken ct = default);

        Task MatchChatAsync(
            Guid chatId, 
            StartSearchChatAction action, 
            CancellationToken ct = default);

        Task<bool> IsSearching(Guid userId,
            CancellationToken ct);

        Task CancelSearch(Guid userId,
            CancellationToken ct);
    }
}
