using AIChatWebServer.Models.Ranks;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class PointTransactionsRepository : BaseRepository, IPointTransactionsRepository
    {
        private readonly ILogger<PointTransactionsRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly NpgsqlConnection? _conn;
        private readonly NpgsqlTransaction? _tx;
        private readonly bool _isExternalConnection;

        public PointTransactionsRepository(IConfiguration configuration,
            ILogger<PointTransactionsRepository> logger) : base(configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _isExternalConnection = false;
        }

        private PointTransactionsRepository(IConfiguration configuration,
            ILogger<PointTransactionsRepository> logger,
            NpgsqlConnection conn,
            NpgsqlTransaction tx) : base(configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _conn = conn;
            _tx = tx;
            _isExternalConnection = true;
        }

        public IPointTransactionsRepository WithTransaction(NpgsqlConnection conn, NpgsqlTransaction tx)
            => new PointTransactionsRepository(_configuration, _logger, conn, tx);

        public async Task<PointTransaction> CreateAsync(Guid userId, int amount, string type, string? reason = null, Guid? referenceId = null, CancellationToken ct = default)
        {
            var list = await ReadAsync(PointTransactionsQueries.Create, ct,
                ("@user_id", userId),
                ("@amount", amount),
                ("@type", type),
                ("@reason", reason ?? (object)DBNull.Value),
                ("@reference_id", referenceId ?? (object)DBNull.Value));
            return list.First();
        }

        public async Task<List<PointTransaction>> GetByUserIdAsync(Guid userId, int limit, int offset, CancellationToken ct = default)
            => await ReadAsync(PointTransactionsQueries.GetByUserId, ct,
                ("@user_id", userId), ("@limit", limit), ("@offset", offset));

        public async Task<List<PointTransaction>> GetByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken ct = default)
            => await ReadAsync(PointTransactionsQueries.GetByUserIdAndDateRange, ct,
                ("@user_id", userId), ("@start_date", startDate), ("@end_date", endDate));

        public async Task<PointTransaction?> GetByReferenceIdAsync(Guid referenceId, CancellationToken ct = default)
        {
            var list = await ReadAsync(PointTransactionsQueries.GetByReferenceId, ct, ("@reference_id", referenceId));
            return list.FirstOrDefault();
        }

        public async Task<int> GetSumByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken ct = default)
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
                    PointTransactionsQueries.GetSumByUserIdAndDateRange,
                    connection,
                    _tx);
                cmd.Parameters.AddWithValue("@user_id", userId);
                cmd.Parameters.AddWithValue("@start_date", startDate);
                cmd.Parameters.AddWithValue("@end_date", endDate);

                var result = await cmd.ExecuteScalarAsync(ct);
                return result == DBNull.Value ? 0 : Convert.ToInt32(result);
            }
            finally
            {
                if (ownsConnection && connection != null)
                {
                    await connection.DisposeAsync();
                }
            }
        }

        public async Task<List<PointTransaction>> GetByTypeAsync(Guid userId, string type, CancellationToken ct = default)
            => await ReadAsync(PointTransactionsQueries.GetByType, ct, ("@user_id", userId), ("@type", type));

        public async Task<int> GetUserBalanceAtDateAsync(Guid userId, DateTime date, CancellationToken ct = default)
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
                    PointTransactionsQueries.GetUserBalanceAtDate,
                    connection,
                    _tx);
                cmd.Parameters.AddWithValue("@user_id", userId);
                cmd.Parameters.AddWithValue("@date", date);

                var result = await cmd.ExecuteScalarAsync(ct);
                return result == DBNull.Value ? 0 : Convert.ToInt32(result);
            }
            finally
            {
                if (ownsConnection && connection != null)
                {
                    await connection.DisposeAsync();
                }
            }
        }

        private async Task<List<PointTransaction>> ReadAsync(
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

                var list = new List<PointTransaction>();

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

        private static PointTransaction Map(NpgsqlDataReader reader)
        {
            return new PointTransaction
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                UserId = reader.GetGuid(reader.GetOrdinal("user_id")),
                Amount = reader.GetInt32(reader.GetOrdinal("amount")),
                Type = reader.GetString(reader.GetOrdinal("type")),
                Reason = reader.IsDBNull(reader.GetOrdinal("reason")) ? null : reader.GetString(reader.GetOrdinal("reason")),
                ReferenceId = reader.IsDBNull(reader.GetOrdinal("reference_id")) ? null : reader.GetGuid(reader.GetOrdinal("reference_id")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
            };
        }
    }
}