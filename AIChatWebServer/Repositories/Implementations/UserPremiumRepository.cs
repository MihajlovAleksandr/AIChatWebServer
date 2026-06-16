using AIChatWebServer.Models.User;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class UserPremiumRepository : BaseRepository, IUserPremiumRepository
    {
        private readonly ILogger<UserPremiumRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly NpgsqlConnection? _conn;
        private readonly NpgsqlTransaction? _tx;
        private readonly bool _isExternalConnection;

        public UserPremiumRepository(IConfiguration configuration,
            ILogger<UserPremiumRepository> logger) : base(configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _isExternalConnection = false;
        }

        private UserPremiumRepository(IConfiguration configuration,
            ILogger<UserPremiumRepository> logger,
            NpgsqlConnection conn,
            NpgsqlTransaction tx) : base(configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _conn = conn;
            _tx = tx;
            _isExternalConnection = true;
        }

        public IUserPremiumRepository WithTransaction(NpgsqlConnection conn, NpgsqlTransaction tx)
            => new UserPremiumRepository(_configuration, _logger, conn, tx);

        public async Task CreateAsync(
            Guid userId,
            Guid paymentItemId,
            DateTime startAt,
            DateTime endAt,
            bool isAutoRenew = false,
            string? subscriptionId = null,
            CancellationToken ct = default)
        {
            await ExecuteAsync(
                UserPremiumQueries.Create,
                ct,
                ("@id", Guid.NewGuid()),
                ("@user_id", userId),
                ("@payment_item_id", paymentItemId),
                ("@start_at", startAt),
                ("@end_at", endAt),
                ("@is_auto_renew", isAutoRenew),
                ("@subscription_id", (object?)subscriptionId ?? DBNull.Value));
        }

        public async Task<UserPremium?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var list = await ReadAsync(
                UserPremiumQueries.GetById,
                ct,
                ("@id", id));

            return list.FirstOrDefault();
        }

        public async Task<List<UserPremium>> GetHistoryAsync(Guid userId, CancellationToken ct = default)
        {
            return await ReadAsync(
                UserPremiumQueries.GetByUser,
                ct,
                ("@user_id", userId));
        }

        public async Task<UserPremium?> GetActiveAsync(Guid userId, CancellationToken ct = default)
        {
            var list = await ReadAsync(
                UserPremiumQueries.GetActive,
                ct,
                ("@user_id", userId));

            return list.FirstOrDefault();
        }

        public async Task<UserPremium?> GetLastAsync(Guid userId, CancellationToken ct = default)
        {
            var list = await ReadAsync(
                UserPremiumQueries.GetLast,
                ct,
                ("@user_id", userId));

            return list.FirstOrDefault();
        }

        public async Task<Guid?> GetUserIdBySubscriptionIdAsync(
            string subscriptionId,
            CancellationToken ct = default)
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

                await using var cmd = new NpgsqlCommand(
                    UserPremiumQueries.GetUserIdBySubscriptionId,
                    connection,
                    _tx);

                cmd.Parameters.AddWithValue("@subscription_id", subscriptionId);

                var result = await cmd.ExecuteScalarAsync(ct);

                if (result == null || result == DBNull.Value)
                    return null;

                return (Guid)result;
            }
            finally
            {
                if (ownsConnection && connection != null)
                {
                    await connection.DisposeAsync();
                }
            }
        }

        public async Task<UserPremium?> GetAutoRenewAsync(Guid userId, CancellationToken ct = default)
        {
            var list = await ReadAsync(
                           UserPremiumQueries.GetAutoRenew,
                           ct,
                           ("@userId", userId));

            return list.FirstOrDefault();
        }

        public async Task CancelAutoRenew(string subscriptionId, CancellationToken ct = default)
        {
            await ExecuteAsync(
                UserPremiumQueries.CancelAutoRenew,
                ct,
                ("@subscriptionId", subscriptionId));
        }

        public async Task<UserPremium?> GetFirstBySubscriptionIdAsync(
            string subscriptionId,
            CancellationToken ct = default)
        {
            var list = await ReadAsync(
                UserPremiumQueries.GetFirstBySubscriptionId,
                ct,
                ("@subscription_id", subscriptionId));

            return list.FirstOrDefault();
        }

        private async Task<List<UserPremium>> ReadAsync(
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
                    cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);

                await using var reader = await cmd.ExecuteReaderAsync(ct);

                var list = new List<UserPremium>();

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
                    cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);

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

        private static UserPremium Map(NpgsqlDataReader reader)
        {
            return new UserPremium
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                StartTime = reader.GetDateTime(reader.GetOrdinal("start_at")),
                EndTime = reader.GetDateTime(reader.GetOrdinal("end_at")),
                IsAutoRenew = reader.GetBoolean(reader.GetOrdinal("is_auto_renew")),
                SubscriptionId = reader.IsDBNull(reader.GetOrdinal("subscription_id"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("subscription_id"))
            };
        }
    }
}