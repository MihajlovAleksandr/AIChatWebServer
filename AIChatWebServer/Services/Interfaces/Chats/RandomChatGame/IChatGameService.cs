using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Chats.RandomChat;

namespace AIChatWebServer.Services.Interfaces.Chats.RandomChatGame
{
    public interface IChatGameService
    {
        Task<ChatGameSession> CreateGameAsync(
            Guid chatId,
            Guid guesserUserChatId,
            Guid opponentUserChatId,
            AiRole opponentAiRole,
            CancellationToken ct = default);

        Task<ChatGameSession> GetByChatIdAsync(
            Guid chatId,
            CancellationToken ct = default);

        Task<ChatGameSession> GetByIdAsync(
            Guid sessionId,
            CancellationToken ct = default);

        Task<ChatGameSession> MakeGuessAsync(
            Guid chatId,
            AiRole guessedAiRole,
            CancellationToken ct = default);

        Task<ChatGameState> GetResultAsync(
            Guid sessionId,
            CancellationToken ct = default);
    }
}