using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Chats.Matchmaking;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class MatchmakingRepository
        : BaseRepository, IMatchmakingRepository
    {
        private readonly NpgsqlConnection? _conn;
        private readonly NpgsqlTransaction? _tx;

        public MatchmakingRepository()
        {
        }

        private MatchmakingRepository(
            NpgsqlConnection conn,
            NpgsqlTransaction tx)
        {
            _conn = conn;
            _tx = tx;
        }

        public IMatchmakingRepository WithTransaction(
            NpgsqlConnection conn,
            NpgsqlTransaction tx)
        {
            return new MatchmakingRepository(conn, tx);
        }

        public async Task<Guid> EnqueueAsync(
            Guid userId,
            ChatType chatType,
            string matchPredicate,
            string? chatName,
            DateTime? expiresAt,
            CancellationToken ct = default)
        {
            var useExternalConnection = _conn != null;

            await using var conn = useExternalConnection
                ? null
                : await GetConnectionAsync(ct);

            var actualConn = _conn ?? conn!;

            await using var cmd =
                new NpgsqlCommand(
                    MatchmakingQueries.Enqueue,
                    actualConn,
                    _tx);

            var id = Guid.NewGuid();

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@chatType", chatType.ToString());
            cmd.Parameters.AddWithValue("@chatName",
                chatName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@predicate", matchPredicate);
            cmd.Parameters.AddWithValue("@expiresAt",
                expiresAt ?? (object)DBNull.Value);

            await cmd.ExecuteNonQueryAsync(ct);

            return id;
        }

        public async Task<MatchmakingEntry?> LockEntryAsync(
            Guid entryId,
            CancellationToken ct = default)
        {
            var list =
                await ReadAsync(
                    MatchmakingQueries.LockEntry,
                    ct,
                    ("@id", entryId));

            return list.FirstOrDefault();
        }

        public async Task<IReadOnlyList<MatchmakingEntry>> AcquireCandidatesAsync(
            Guid userId,
            ChatType chatType,
            int limit,
            CancellationToken ct = default)
        {
            return await ReadAsync(
                MatchmakingQueries.AcquireCandidates,
                ct,
                ("@userId", userId),
                ("@chatType", chatType.ToString()),
                ("@limit", limit));
        }

        public async Task CompleteAsync(
            Guid[] entries,
            CancellationToken ct = default)
        {
            await ExecuteAsync(
                MatchmakingQueries.Complete,
                ct,
                ("ids", entries)
            );
        }

        public Task CancelAsync(
            Guid entryId,
            CancellationToken ct = default) =>
            ExecuteAsync(
                MatchmakingQueries.Cancel,
                ct,
                ("@id", entryId));

        public async Task<MatchmakingEntry?> GetChatByUserAsync(
            Guid userId,
            CancellationToken ct = default)
        {
            var list =
                await ReadAsync(
                    MatchmakingQueries.GetChatByUser,
                    ct,
                    ("@userId", userId));

            return list.FirstOrDefault();
        }

        public async Task<MatchmakingEntry?> GetGroupByUserAsync(
            Guid userId,
            CancellationToken ct = default)
        {
            var list =
                await ReadAsync(
                    MatchmakingQueries.GetGroupByUser,
                    ct,
                    ("@userId", userId));

            return list.FirstOrDefault();
        }

        public Task ExpireAsync(
            CancellationToken ct = default) =>
            ExecuteAsync(
                MatchmakingQueries.Expire,
                ct);

        private async Task<List<MatchmakingEntry>> ReadAsync(
            string sql,
            CancellationToken ct,
            params (string, object)[] parameters)
        {
            var list =
                new List<MatchmakingEntry>();

            if (_conn != null)
            {
                await using var cmd =
                    new NpgsqlCommand(sql, _conn, _tx);

                foreach (var (n, v) in parameters)
                {
                    if (v is NpgsqlParameter p)
                        cmd.Parameters.Add(p);
                    else
                        cmd.Parameters.AddWithValue(
                            n,
                            v ?? DBNull.Value);
                }

                await using var r =
                    await cmd.ExecuteReaderAsync(ct);

                while (await r.ReadAsync(ct))
                    list.Add(Map(r));

                return list;
            }

            await using var conn =
                await GetConnectionAsync(ct);

            await using var cmd2 =
                new NpgsqlCommand(sql, conn);

            foreach (var (n, v) in parameters)
            {
                if (v is NpgsqlParameter p)
                    cmd2.Parameters.Add(p);
                else
                    cmd2.Parameters.AddWithValue(
                        n,
                        v ?? DBNull.Value);
            }

            await using var r2 =
                await cmd2.ExecuteReaderAsync(ct);

            while (await r2.ReadAsync(ct))
                list.Add(Map(r2));

            return list;
        }

        private async Task ExecuteAsync(
            string sql,
            CancellationToken ct,
            params (string, object)[] parameters)
        {
            if (_conn != null)
            {
                await using var cmd =
                    new NpgsqlCommand(sql, _conn, _tx);

                foreach (var (n, v) in parameters)
                {
                    if (v is NpgsqlParameter p)
                        cmd.Parameters.Add(p);
                    else
                        cmd.Parameters.AddWithValue(
                            n,
                            v ?? DBNull.Value);
                }

                await cmd.ExecuteNonQueryAsync(ct);
                return;
            }

            await using var conn =
                await GetConnectionAsync(ct);

            await using var cmd2 =
                new NpgsqlCommand(sql, conn);

            foreach (var (n, v) in parameters)
            {
                if (v is NpgsqlParameter p)
                    cmd2.Parameters.Add(p);
                else
                    cmd2.Parameters.AddWithValue(
                        n,
                        v ?? DBNull.Value);
            }

            await cmd2.ExecuteNonQueryAsync(ct);
        }

        private static MatchmakingEntry Map(
            NpgsqlDataReader r)
        {
            return new MatchmakingEntry(
                r.GetGuid(r.GetOrdinal("id")),
                r.GetGuid(r.GetOrdinal("user_id")),
                Enum.Parse<ChatType>(
                    r.GetString(r.GetOrdinal("chat_type"))),
                r.GetString(r.GetOrdinal("chat_name")),
                r.GetString(r.GetOrdinal("match_predicate")),
                (ChatMatchStatus)r.GetInt16(r.GetOrdinal("status")),
                r.GetDateTime(r.GetOrdinal("created_at")),
                r.IsDBNull(r.GetOrdinal("expires_at"))
                    ? null
                    : r.GetDateTime(r.GetOrdinal("expires_at"))
            );
        }
    }
}