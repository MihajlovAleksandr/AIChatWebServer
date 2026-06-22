using AIChatWebServer.Models.Chats;

namespace AIChatWebServer.Services.Interfaces.Chats.Matchmaking
{
    public interface IChatCreateStrategiesHandler
    {
        Task<Guid> CreateAsync(
            ChatType chatType,
            Guid userId,
            string chatName,
            CancellationToken ct);
    }
}
