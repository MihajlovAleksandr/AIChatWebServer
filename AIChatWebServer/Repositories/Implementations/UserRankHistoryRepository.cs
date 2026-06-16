using AIChatWebServer.Models.Ranks;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class UserRankHistoryRepository : BaseRepository, IUserRankHistoryRepository
    {
        private readonly ILogger<UserRankHistoryRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly NpgsqlConnection? _conn;
        private readonly NpgsqlTransaction? _tx;
        private readonly bool _isExternalConnection;

        public UserRankHistoryRepository(IConfiguration configuration,
            ILogger<UserRankHistoryRepository> logger) : base(configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _isExternalConnection = false;
        }

        private UserRankHistoryRepository(IConfiguration configuration,
            ILogger<UserRankHistoryRepository> logger,
            NpgsqlConnection conn,
            NpgsqlTransaction tx) : base(configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _conn = conn;
            _tx = tx;
            _isExternalConnection = true;
        }

        public IUserRankHistoryRepository WithTransaction(NpgsqlConnection conn, NpgsqlTransaction tx)
            => new UserRankHistoryRepository(_configuration, _logger, conn, tx);

        public async Task<UserRankHistory> CreateAsync(Guid userId, int rankId, int pointsAtMoment, CancellationToken ct = default)
        {
            var list = await ReadHistoryAsync(UserRankHistoryQueries.Create, ct,
                ("@user_id", userId), ("@rank_id", rankId), ("@points_at_moment", pointsAtMoment));
            return list.First();
        }

        public async Task<UserRankWithDetails?> GetCurrentRankByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            var list = await ReadWithDetailsAsync(UserRankHistoryQueries.GetCurrentRankByUserId, ct, ("@user_id", userId));
            return list.FirstOrDefault();
        }

        public async Task<List<UserRankWithDetails>> GetHistoryByUserIdAsync(Guid userId, int limit, int offset, CancellationToken ct = default)
            => await ReadWithDetailsAsync(UserRankHistoryQueries.GetHistoryByUserId, ct,
                ("@user_id", userId), ("@limit", limit), ("@offset", offset));

        public async Task<List<Guid>> GetUsersWhoChangedRankBetweenDatesAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default)
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
                    UserRankHistoryQueries.GetUsersWhoChangedRankBetweenDates,
                    connection,
                    _tx);
                cmd.Parameters.AddWithValue("@start_date", startDate);
                cmd.Parameters.AddWithValue("@end_date", endDate);

                var result = new List<Guid>();
                await using var reader = await cmd.ExecuteReaderAsync(ct);

                while (await reader.ReadAsync(ct))
                    result.Add(reader.GetGuid(reader.GetOrdinal("user_id")));

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

        public async Task<UserRankHistory?> GetLastRankChangeByUserAsync(Guid userId, CancellationToken ct = default)
        {
            var list = await ReadHistoryAsync(UserRankHistoryQueries.GetLastRankChangeByUser, ct, ("@user_id", userId));
            return list.FirstOrDefault();
        }

        public async Task<List<UserRankWithDetails>> GetRankHistoryByDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken ct = default)
            => await ReadWithDetailsAsync(UserRankHistoryQueries.GetRankHistoryByDateRange, ct,
                ("@user_id", userId), ("@start_date", startDate), ("@end_date", endDate));

        public async Task<List<Guid>> GetUsersInRankAsync(int rankId, CancellationToken ct = default)
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
                    UserRankHistoryQueries.GetUsersInRank,
                    connection,
                    _tx);
                cmd.Parameters.AddWithValue("@rank_id", rankId);

                var result = new List<Guid>();
                await using var reader = await cmd.ExecuteReaderAsync(ct);

                while (await reader.ReadAsync(ct))
                    result.Add(reader.GetGuid(reader.GetOrdinal("user_id")));

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

        private async Task<List<UserRankHistory>> ReadHistoryAsync(
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

                var list = new List<UserRankHistory>();

                while (await reader.ReadAsync(ct))
                    list.Add(MapHistory(reader));

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

        private async Task<List<UserRankWithDetails>> ReadWithDetailsAsync(
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

                var list = new List<UserRankWithDetails>();

                while (await reader.ReadAsync(ct))
                    list.Add(MapWithDetails(reader));

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

        private static UserRankHistory MapHistory(NpgsqlDataReader reader)
        {
            return new UserRankHistory
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                UserId = reader.GetGuid(reader.GetOrdinal("user_id")),
                RankId = reader.GetInt32(reader.GetOrdinal("rank_id")),
                ChangedAt = reader.GetDateTime(reader.GetOrdinal("changed_at")),
                PointsAtMoment = reader.GetInt32(reader.GetOrdinal("points_at_moment"))
            };
        }

        private static UserRankWithDetails MapWithDetails(NpgsqlDataReader reader)
        {
            return new UserRankWithDetails
            {
                UserId = reader.GetGuid(reader.GetOrdinal("user_id")),
                RankId = reader.GetInt32(reader.GetOrdinal("rank_id")),
                RankName = reader.GetString(reader.GetOrdinal("rank_name")),
                MinPoints = reader.GetInt32(reader.GetOrdinal("min_points")),
                MaxPoints = reader.IsDBNull(reader.GetOrdinal("max_points")) ? null : reader.GetInt32(reader.GetOrdinal("max_points")),
                Priority = reader.GetInt32(reader.GetOrdinal("priority")),
                ChangedAt = reader.GetDateTime(reader.GetOrdinal("changed_at")),
                PointsAtMoment = reader.GetInt32(reader.GetOrdinal("points_at_moment"))
            };
        }
    }
}