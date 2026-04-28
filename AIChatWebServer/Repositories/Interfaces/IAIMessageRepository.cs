using AIChatWebServer.Integrations.AI;
using AIChatWebServer.Models.AI;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IAIMessageRepository
    {
        Task<AIMessage> Add(
            Guid chatId,
            AIMessageRole role,
            AIMessageType type,
            string content,
            CancellationToken cancellationToken = default);

        Task<AIMessage?> GetById(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<AIMessage>> GetByChatId(
            Guid chatId,
            CancellationToken cancellationToken = default);

        Task<bool> Delete(
            Guid id,
            CancellationToken cancellationToken = default);

        Task UseTokens(
            Guid chatId, 
            int tokensCount, 
            AIModel model,
            TokenOperation operation,
            CancellationToken cancellationToken = default);
    }
}