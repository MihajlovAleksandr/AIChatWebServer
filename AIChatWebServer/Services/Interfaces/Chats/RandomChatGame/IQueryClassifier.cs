using AIChatWebServer.Models.Chats.RandomChat;

namespace AIChatWebServer.Services.Interfaces.Chats.RandomChatGame
{
    public interface IQueryClassifier
    {
        Task<QueryTags> ClassifyAsync(
            Guid chatId,
            string message,
            CancellationToken ct = default);
    }
}
