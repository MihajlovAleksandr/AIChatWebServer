using AIChatWebServer.Models.Chats;

namespace AIChatWebServer.Services.Interfaces.Chats.Matchmaking
{
    public interface IChatMatchStrategy
    {
        ChatType MatchType { get; }

        Task MatchUserAsync(
            Guid userId,
            string userPredicate,
            string chatName,
            CancellationToken ct);
    }
}
