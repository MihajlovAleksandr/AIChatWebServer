using AIChatWebServer.Contracts.UnitOfWork.Interfaces;
using AIChatWebServer.Models.Chats.Matchmaking;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IGroupChatSearchRepository : ITransactionalScope<IGroupChatSearchRepository>
    {
        Task<Guid> EnqueueAsync(
            Guid chatId,
            Guid userId,
            string matchPredicate,
            int slots,
            CancellationToken ct = default);

        Task<GroupChatSearchEntry?> LockEntryAsync(
            Guid id,
            CancellationToken ct = default);

        Task<IReadOnlyList<GroupChatSearchEntry>> AcquireCandidatesAsync(
            Guid userId,
            int limit,
            CancellationToken ct = default);

        Task CompleteAsync(
            Guid[] ids,
            CancellationToken ct = default);

        Task CancelAsync(
            Guid id,
            CancellationToken ct = default);

        Task<GroupChatSearchEntry?> GetByUserAsync(
            Guid userId,
            CancellationToken ct = default);
    }
}