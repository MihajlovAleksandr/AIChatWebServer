using AIChatWebServer.Models.Chats;

namespace AIChatWebServer.Services.Interfaces.Chats
{
    public interface IRandomChatService
    {
        Task Create(Guid first, Guid second, Guid chatId, CancellationToken ct);
        event Action<Chat>? OnChatEnded;
    }
}
