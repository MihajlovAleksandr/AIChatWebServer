using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Chats.Matchmaking;
using AIChatWebServer.Repositories.Constants;
using Npgsql;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IMatchmakingRepository
    {
        IMatchmakingRepository WithTransaction(
            NpgsqlConnection conn,
            NpgsqlTransaction tx);

        Task<Guid> EnqueueAsync(
            Guid userId,
            ChatType chatType,
            string matchPredicate,
            string? chatName,
            DateTime? expiresAt,
            CancellationToken ct = default);

        Task<MatchmakingEntry?> LockEntryAsync(
            Guid entryId,
            CancellationToken ct = default);

        Task<IReadOnlyList<MatchmakingEntry>> AcquireCandidatesAsync(
            Guid userId,
            ChatType chatType,
            int limit,
            CancellationToken ct = default);

        Task CompleteAsync(
            Guid[] entries,
            CancellationToken ct = default);

        Task CancelAsync(
            Guid entryId,
            CancellationToken ct = default);

        Task<MatchmakingEntry?> GetChatByUserAsync(
                   Guid userId,
                   CancellationToken ct = default);

        Task<MatchmakingEntry?> GetGroupByUserAsync(
            Guid userId,
            CancellationToken ct = default);

        Task ExpireAsync(
            CancellationToken ct = default);
    }
}