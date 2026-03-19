using AIChatWebServer.Models.Chats.Matchmaking;
using Npgsql;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IGroupChatSearchRepository
    {
        IGroupChatSearchRepository WithTransaction(
            NpgsqlConnection conn,
            NpgsqlTransaction tx);

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