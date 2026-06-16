using AIChatWebServer.Models.Ranks;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class UserPointsRepository : BaseRepository, IUserPointsRepository
    {
        private readonly ILogger<UserPointsRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly NpgsqlConnection? _conn;
        private readonly NpgsqlTransaction? _tx;
        private readonly bool _isExternalConnection;

        public UserPointsRepository(IConfiguration configuration,
            ILogger<UserPointsRepository> logger) : base(configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _isExternalConnection = false;
        }

        private UserPointsRepository(IConfiguration configuration,
            ILogger<UserPointsRepository> logger,
            NpgsqlConnection conn,
            NpgsqlTransaction tx) : base(configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _conn = conn;
            _tx = tx;
            _isExternalConnection = true;
        }

        public IUserPointsRepository WithTransaction(NpgsqlConnection conn, NpgsqlTransaction tx)
            => new UserPointsRepository(_configuration, _logger, conn, tx);

        public async Task<UserPoints> FindOrCreateByUserIdAsync(Guid userId, CancellationToken ct = default)
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

                await using var cmd = new NpgsqlCommand(UserPointsQueries.FindOrCreateByUserId, connection, _tx);
                cmd.Parameters.AddWithValue("@user_id", userId);

                await using var reader = await cmd.ExecuteReaderAsync(ct);

                if (await reader.ReadAsync(ct))
                    return Map(reader);

                await reader.CloseAsync();

                await using var cmd2 = new NpgsqlCommand(UserPointsQueries.FindOrCreateByUserIdSelect, connection, _tx);
                cmd2.Parameters.AddWithValue("@user_id", userId);

                await using var reader2 = await cmd2.ExecuteReaderAsync(ct);

                if (await reader2.ReadAsync(ct))
                    return Map(reader2);

                throw new InvalidOperationException($"Failed to create or find user points for user: {userId}");
            }
            finally
            {
                if (ownsConnection && connection != null)
                {
                    await connection.DisposeAsync();
                }
            }
        }

        public async Task<UserPoints?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            var list = await ReadAsync(UserPointsQueries.GetByUserId, ct, ("@user_id", userId));
            return list.FirstOrDefault();
        }

        public async Task<UserPoints> UpdateTotalPointsAsync(Guid userId, int totalPoints, CancellationToken ct = default)
        {
            var list = await ReadAsync(UserPointsQueries.UpdateTotalPoints, ct,
                ("@user_id", userId), ("@total_points", totalPoints));
            return list.First();
        }

        public async Task<UserPoints> IncrementPointsAsync(Guid userId, int amount, CancellationToken ct = default)
        {
            var list = await ReadAsync(UserPointsQueries.IncrementPoints, ct,
                ("@user_id", userId), ("@amount", amount));
            return list.First();
        }

        public async Task<UserPoints> DecrementPointsAsync(Guid userId, int amount, CancellationToken ct = default)
        {
            var list = await ReadAsync(UserPointsQueries.DecrementPoints, ct,
                ("@user_id", userId), ("@amount", amount));
            return list.First();
        }

        public async Task<List<UserPoints>> GetByPointsRangeAsync(int minPoints, int maxPoints, CancellationToken ct = default)
            => await ReadAsync(UserPointsQueries.GetByPointsRange, ct, ("@min_points", minPoints), ("@max_points", maxPoints));

        public async Task<List<UserPoints>> GetLeaderboardAsync(int limit, int offset, CancellationToken ct = default)
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

                await using var cmd = new NpgsqlCommand(UserPointsQueries.GetLeaderboard, connection, _tx);
                cmd.Parameters.AddWithValue("@limit", limit);
                cmd.Parameters.AddWithValue("@offset", offset);

                await using var reader = await cmd.ExecuteReaderAsync(ct);

                var result = new List<UserPoints>();

                while (await reader.ReadAsync(ct))
                {
                    result.Add(Map(reader));
                }

                return result;
            }
            finally
            {
                if (ownsConnection && connection != null)
                {
                    await connection.DisposeAsync();
                }
            }
        }

        public async Task<int> GetLeaderboardTotalCountAsync(CancellationToken ct = default)
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

                await using var cmd = new NpgsqlCommand(UserPointsQueries.GetLeaderboardTotalCount, connection, _tx);

                var result = await cmd.ExecuteScalarAsync(ct);
                return Convert.ToInt32(result);
            }
            finally
            {
                if (ownsConnection && connection != null)
                {
                    await connection.DisposeAsync();
                }
            }
        }

        private async Task<List<UserPoints>> ReadAsync(
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

                var list = new List<UserPoints>();

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

        private static UserPoints Map(NpgsqlDataReader reader)
        {
            return new UserPoints
            {
                UserId = reader.GetGuid(reader.GetOrdinal("user_id")),
                TotalPoints = reader.GetInt32(reader.GetOrdinal("total_points")),
                LastUpdated = reader.GetDateTime(reader.GetOrdinal("last_updated"))
            };
        }
    }
}