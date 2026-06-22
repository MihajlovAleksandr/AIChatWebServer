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
        private readonly ILogger<MatchmakingRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly NpgsqlConnection? _conn;
        private readonly NpgsqlTransaction? _tx;
        private readonly bool _isExternalConnection;

        public MatchmakingRepository(IConfiguration configuration,
            ILogger<MatchmakingRepository> logger) : base(configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _isExternalConnection = false;
        }

        private MatchmakingRepository(IConfiguration configuration,
            ILogger<MatchmakingRepository> logger,
            NpgsqlConnection conn,
            NpgsqlTransaction tx) : base(configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _conn = conn;
            _tx = tx;
            _isExternalConnection = true;
        }

        public IMatchmakingRepository WithTransaction(
            NpgsqlConnection conn,
            NpgsqlTransaction tx)
        {
            return new MatchmakingRepository(_configuration, _logger, conn, tx);
        }

        public async Task<Guid> EnqueueAsync(
            Guid userId,
            ChatType chatType,
            string matchPredicate,
            string? chatName,
            DateTime? expiresAt,
            CancellationToken ct = default)
        {
            var id = Guid.NewGuid();
            NpgsqlConnection? connection = null;
            NpgsqlTransaction? transaction = null;
            bool ownsConnection = false;
            bool ownsTransaction = false;

            try
            {
                if (_isExternalConnection)
                {
                    connection = _conn;
                    transaction = _tx;
                }
                else
                {
                    connection = await GetConnectionAsync(ct);
                    transaction = await connection.BeginTransactionAsync(ct);
                    ownsConnection = true;
                    ownsTransaction = true;
                }

                await using var cmd = new NpgsqlCommand(
                    MatchmakingQueries.Enqueue,
                    connection,
                    transaction);

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@chatType", chatType.ToString());
                cmd.Parameters.AddWithValue("@chatName",
                    chatName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@predicate", matchPredicate);
                cmd.Parameters.AddWithValue("@expiresAt",
                    expiresAt ?? (object)DBNull.Value);

                await cmd.ExecuteNonQueryAsync(ct);

                if (ownsTransaction && transaction != null)
                {
                    await transaction.CommitAsync(ct);
                }

                return id;
            }
            catch (Exception ex)
            {
                if (ownsTransaction && transaction != null)
                {
                    await transaction.RollbackAsync(ct);
                }

                _logger.LogError(ex,
                    "Failed to enqueue matchmaking for UserId={UserId}, ChatType={ChatType}",
                    userId, chatType);
                throw;
            }
            finally
            {
                if (ownsConnection && connection != null)
                {
                    await connection.DisposeAsync();
                }
            }
        }

        public async Task<MatchmakingEntry?> LockEntryAsync(
            Guid entryId,
            CancellationToken ct = default)
        {
            var list = await ReadAsync(
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
            CancellationToken ct = default)
        {
            return ExecuteAsync(
                MatchmakingQueries.Cancel,
                ct,
                ("@id", entryId));
        }

        public async Task<MatchmakingEntry?> GetChatByUserAsync(
            Guid userId,
            CancellationToken ct = default)
        {
            var list = await ReadAsync(
                MatchmakingQueries.GetChatByUser,
                ct,
                ("@userId", userId));

            return list.FirstOrDefault();
        }

        public async Task<MatchmakingEntry?> GetGroupByUserAsync(
            Guid userId,
            CancellationToken ct = default)
        {
            var list = await ReadAsync(
                MatchmakingQueries.GetGroupByUser,
                ct,
                ("@userId", userId));

            return list.FirstOrDefault();
        }

        public Task ExpireAsync(
            CancellationToken ct = default)
        {
            return ExecuteAsync(
                MatchmakingQueries.Expire,
                ct);
        }

        private async Task<List<MatchmakingEntry>> ReadAsync(
            string sql,
            CancellationToken ct,
            params (string, object)[] parameters)
        {
            NpgsqlConnection? connection = null;
            bool ownsConnection = false;

            try
            {
                if (_isExternalConnection)
                {
                    connection = _conn;
                }
                else
                {
                    connection = await GetConnectionAsync(ct);
                    ownsConnection = true;
                }

                await using var cmd = new NpgsqlCommand(sql, connection, _tx);

                foreach (var (name, value) in parameters)
                {
                    if (value is NpgsqlParameter p)
                        cmd.Parameters.Add(p);
                    else
                        cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
                }

                await using var reader = await cmd.ExecuteReaderAsync(ct);

                var list = new List<MatchmakingEntry>();

                while (await reader.ReadAsync(ct))
                    list.Add(Map(reader));

                return list;
            }
            finally
            {
                if (ownsConnection && connection != null)
                {
                    await connection.DisposeAsync();
                }
            }
        }

        private async Task ExecuteAsync(
            string sql,
            CancellationToken ct,
            params (string, object)[] parameters)
        {
            NpgsqlConnection? connection = null;
            NpgsqlTransaction? transaction = null;
            bool ownsConnection = false;
            bool ownsTransaction = false;

            try
            {
                if (_isExternalConnection)
                {
                    connection = _conn;
                    transaction = _tx;
                }
                else
                {
                    connection = await GetConnectionAsync(ct);
                    transaction = await connection.BeginTransactionAsync(ct);
                    ownsConnection = true;
                    ownsTransaction = true;
                }

                await using var cmd = new NpgsqlCommand(sql, connection, transaction);

                foreach (var (name, value) in parameters)
                {
                    if (value is NpgsqlParameter p)
                        cmd.Parameters.Add(p);
                    else
                        cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
                }

                await cmd.ExecuteNonQueryAsync(ct);

                if (ownsTransaction && transaction != null)
                {
                    await transaction.CommitAsync(ct);
                }
            }
            catch (Exception ex)
            {
                if (ownsTransaction && transaction != null)
                {
                    await transaction.RollbackAsync(ct);
                }

                _logger.LogError(ex, "Failed to execute query: {Sql}", sql);
                throw;
            }
            finally
            {
                if (ownsConnection && connection != null)
                {
                    await connection.DisposeAsync();
                }
            }
        }

        private static MatchmakingEntry Map(NpgsqlDataReader reader)
        {
            return new MatchmakingEntry(
                reader.GetGuid(reader.GetOrdinal("id")),
                reader.GetGuid(reader.GetOrdinal("user_id")),
                Enum.Parse<ChatType>(
                    reader.GetString(reader.GetOrdinal("chat_type"))),
                reader.GetString(reader.GetOrdinal("chat_name")),
                reader.GetString(reader.GetOrdinal("match_predicate")),
                (ChatMatchStatus)reader.GetInt16(reader.GetOrdinal("status")),
                reader.GetDateTime(reader.GetOrdinal("created_at")),
                reader.IsDBNull(reader.GetOrdinal("expires_at"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("expires_at"))
            );
        }
    }
}