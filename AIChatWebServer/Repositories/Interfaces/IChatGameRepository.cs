using AIChatWebServer.Contracts.UnitOfWork.Interfaces;
using AIChatWebServer.Models.Chats.RandomChat;

namespace AIChatWebServer.Repositories.Interfaces;

public interface IChatGameRepository : ITransactionalScope<IChatGameRepository>
{
    Task<Guid> CreateAsync(
        Guid chatId,
        Guid guesserUserChatId,
        Guid opponentUserChatId,
        AiRole opponentAiRole,
        CancellationToken ct = default);

    Task<ChatGameSession?> GetByIdAsync(
        Guid sessionId,
        CancellationToken ct = default);

    Task<ChatGameSession?> GetByChatIdAsync(
        Guid chatId,
        CancellationToken ct = default);

    Task SetResultAsync(
        Guid sessionId,
        AiRole guessedAiRole,
        CancellationToken ct = default);

    Task<bool?> GetResultAsync(
        Guid sessionId,
        CancellationToken ct = default);
}