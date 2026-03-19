using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Chats.Matchmaking;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;
using NpgsqlTypes;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class GroupChatSearchRepository
        : BaseRepository, IGroupChatSearchRepository
    {
        private readonly NpgsqlConnection? _conn;
        private readonly NpgsqlTransaction? _tx;

        public GroupChatSearchRepository()
        {
        }

        private GroupChatSearchRepository(
            NpgsqlConnection conn,
            NpgsqlTransaction tx)
        {
            _conn = conn;
            _tx = tx;
        }

        public IGroupChatSearchRepository WithTransaction(
            NpgsqlConnection conn,
            NpgsqlTransaction tx)
        {
            return new GroupChatSearchRepository(conn, tx);
        }

        public async Task<Guid> EnqueueAsync(
            Guid chatId,
            Guid userId,
            string matchPredicate,
            int slots,
            CancellationToken ct = default)
        {
            var id = Guid.NewGuid();

            var useExternalConnection = _conn != null;

            await using var conn = useExternalConnection
                ? null
                : await GetConnectionAsync(ct);

            var actualConn = _conn ?? conn!;

            await using var cmd =
                new NpgsqlCommand(
                    GroupChatSearchQueries.Enqueue,
                    actualConn,
                    _tx);

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@chatId", chatId);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@predicate", matchPredicate);
            cmd.Parameters.AddWithValue("@slots", slots);

            await cmd.ExecuteNonQueryAsync(ct);

            return id;
        }

        public async Task<GroupChatSearchEntry?> LockEntryAsync(
            Guid id,
            CancellationToken ct = default)
        {
            var list =
                await ReadAsync(
                    GroupChatSearchQueries.LockEntry,
                    ct,
                    ("@id", id));

            return list.FirstOrDefault();
        }

        public async Task<IReadOnlyList<GroupChatSearchEntry>> AcquireCandidatesAsync(
            Guid userId,
            int limit,
            CancellationToken ct = default)
        {
            return await ReadAsync(
                GroupChatSearchQueries.AcquireCandidates,
                ct,
                ("@userId", userId),
                ("@limit", limit));
        }

        public async Task CompleteAsync(
            Guid[] ids,
            CancellationToken ct = default)
        {
            await ExecuteAsync(
                GroupChatSearchQueries.Complete,
                ct,
                ("@ids", ids));
        }

        public Task CancelAsync(
            Guid id,
            CancellationToken ct = default) =>
            ExecuteAsync(
                GroupChatSearchQueries.Cancel,
                ct,
                ("@id", id));

        public async Task<GroupChatSearchEntry?> GetByUserAsync(
            Guid userId,
            CancellationToken ct = default)
        {
            var list =
                await ReadAsync(
                    GroupChatSearchQueries.GetByUser,
                    ct,
                    ("@userId", userId));

            return list.FirstOrDefault();
        }

        private async Task<List<GroupChatSearchEntry>> ReadAsync(
            string sql,
            CancellationToken ct,
            params (string, object)[] parameters)
        {
            var list =
                new List<GroupChatSearchEntry>();

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

        private static GroupChatSearchEntry Map(
            NpgsqlDataReader r)
        {
            return new GroupChatSearchEntry(
                r.GetGuid(r.GetOrdinal("id")),
                r.GetGuid(r.GetOrdinal("chat_id")),
                r.GetGuid(r.GetOrdinal("user_id")),
                r.GetString(r.GetOrdinal("match_predicate")),
                r.GetInt32(r.GetOrdinal("slots")),
                (ChatMatchStatus)r.GetInt16(r.GetOrdinal("status")),
                r.GetDateTime(r.GetOrdinal("created_at"))
            );
        }
    }
}