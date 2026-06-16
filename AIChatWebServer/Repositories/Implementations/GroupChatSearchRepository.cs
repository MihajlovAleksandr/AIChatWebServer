using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Chats.Matchmaking;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class GroupChatSearchRepository
        : BaseRepository, IGroupChatSearchRepository
    {
        private readonly ILogger<GroupChatSearchRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly NpgsqlConnection? _conn;
        private readonly NpgsqlTransaction? _tx;
        private readonly bool _isExternalConnection;

        public GroupChatSearchRepository(
            IConfiguration configuration,
            ILogger<GroupChatSearchRepository> logger) : base(configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _isExternalConnection = false;
        }

        private GroupChatSearchRepository(
            IConfiguration configuration,
            ILogger<GroupChatSearchRepository> logger,
            NpgsqlConnection conn,
            NpgsqlTransaction tx) : base(configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _conn = conn;
            _tx = tx;
            _isExternalConnection = true;
        }

        public IGroupChatSearchRepository WithTransaction(
            NpgsqlConnection conn,
            NpgsqlTransaction tx)
        {
            return new GroupChatSearchRepository(_configuration, _logger, conn, tx);
        }

        public async Task<Guid> EnqueueAsync(
            Guid chatId,
            Guid userId,
            string matchPredicate,
            int slots,
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
                    GroupChatSearchQueries.Enqueue,
                    connection,
                    transaction);

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@chatId", chatId);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@predicate", matchPredicate);
                cmd.Parameters.AddWithValue("@slots", slots);

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
                    "Failed to enqueue group chat search for ChatId={ChatId}, UserId={UserId}",
                    chatId, userId);
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

        public async Task<GroupChatSearchEntry?> LockEntryAsync(
            Guid id,
            CancellationToken ct = default)
        {
            var list = await ReadAsync(
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
            CancellationToken ct = default)
        {
            return ExecuteAsync(
                GroupChatSearchQueries.Cancel,
                ct,
                ("@id", id));
        }

        public async Task<GroupChatSearchEntry?> GetByUserAsync(
            Guid userId,
            CancellationToken ct = default)
        {
            var list = await ReadAsync(
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

                var list = new List<GroupChatSearchEntry>();

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

        private static GroupChatSearchEntry Map(NpgsqlDataReader reader)
        {
            return new GroupChatSearchEntry(
                reader.GetGuid(reader.GetOrdinal("id")),
                reader.GetGuid(reader.GetOrdinal("chat_id")),
                reader.GetGuid(reader.GetOrdinal("user_id")),
                reader.GetString(reader.GetOrdinal("match_predicate")),
                reader.GetInt32(reader.GetOrdinal("slots")),
                (ChatMatchStatus)reader.GetInt16(reader.GetOrdinal("status")),
                reader.GetDateTime(reader.GetOrdinal("created_at"))
            );
        }
    }
}