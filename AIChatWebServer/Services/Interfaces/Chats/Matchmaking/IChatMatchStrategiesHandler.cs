using AIChatWebServer.Models.Chats;

namespace AIChatWebServer.Services.Interfaces.Chats.Matchmaking
{
    public interface IChatMatchStrategiesHandler
    {
        Task MatchUserAsync(
            ChatType chatType,
            Guid userId,
            string userPredicate,
            string chatName,
            CancellationToken ct);
    }
}
