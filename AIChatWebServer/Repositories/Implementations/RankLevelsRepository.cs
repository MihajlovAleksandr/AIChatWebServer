using AIChatWebServer.Models.Ranks;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class RankLevelsRepository : BaseRepository, IRankLevelsRepository
    {
        private readonly ILogger<RankLevelsRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly NpgsqlConnection? _conn;
        private readonly NpgsqlTransaction? _tx;
        private readonly bool _isExternalConnection;

        public RankLevelsRepository(IConfiguration configuration,
            ILogger<RankLevelsRepository> logger) : base(configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _isExternalConnection = false;
        }

        private RankLevelsRepository(IConfiguration configuration,
            ILogger<RankLevelsRepository> logger,
            NpgsqlConnection conn,
            NpgsqlTransaction tx) : base(configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _conn = conn;
            _tx = tx;
            _isExternalConnection = true;
        }

        public IRankLevelsRepository WithTransaction(NpgsqlConnection conn, NpgsqlTransaction tx)
            => new RankLevelsRepository(_configuration, _logger, conn, tx);

        public async Task<RankLevel?> FindRankByPointsAsync(int points, CancellationToken ct = default)
        {
            var list = await ReadAsync(RankLevelsQueries.FindRankByPoints, ct, ("@points", points));
            return list.FirstOrDefault();
        }

        public async Task<RankLevel?> FindRankByPriorityAsync(int priority, CancellationToken ct = default)
        {
            var list = await ReadAsync(RankLevelsQueries.FindRankByPriority, ct, ("@priority", priority));
            return list.FirstOrDefault();
        }

        public async Task<List<RankLevel>> FindAllRanksSortedByPriorityAsync(CancellationToken ct = default)
            => await ReadAsync(RankLevelsQueries.FindAllRanksSortedByPriority, ct);

        public async Task<RankLevel?> FindRankByMinPointsAsync(int minPoints, CancellationToken ct = default)
        {
            var list = await ReadAsync(RankLevelsQueries.FindRankByMinPoints, ct, ("@min_points", minPoints));
            return list.FirstOrDefault();
        }

        public async Task<RankLevel?> GetNextRankAsync(int currentRankId, CancellationToken ct = default)
        {
            var list = await ReadAsync(RankLevelsQueries.GetNextRank, ct, ("@rank_id", currentRankId));
            return list.FirstOrDefault();
        }

        public async Task<RankLevel?> GetPreviousRankAsync(int currentRankId, CancellationToken ct = default)
        {
            var list = await ReadAsync(RankLevelsQueries.GetPreviousRank, ct, ("@rank_id", currentRankId));
            return list.FirstOrDefault();
        }

        public async Task<(RankLevel Current, RankLevel? Next)> GetRankWithNextByPointsAsync(int points, CancellationToken ct = default)
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
                    RankLevelsQueries.GetRankByMinPointsWithNext,
                    connection,
                    _tx);
                cmd.Parameters.AddWithValue("@points", points);

                await using var reader = await cmd.ExecuteReaderAsync(ct);

                if (await reader.ReadAsync(ct))
                {
                    var current = MapRank(reader);
                    RankLevel? next = null;

                    var nextIdOrdinal = reader.GetOrdinal("next_id");
                    if (!reader.IsDBNull(nextIdOrdinal))
                    {
                        next = new RankLevel
                        {
                            Id = reader.GetInt32(nextIdOrdinal),
                            Name = reader.GetString(reader.GetOrdinal("next_name")),
                            MinPoints = reader.GetInt32(reader.GetOrdinal("next_min_points")),
                            MaxPoints = reader.IsDBNull(reader.GetOrdinal("next_max_points")) ? null : reader.GetInt32(reader.GetOrdinal("next_max_points")),
                            Priority = reader.GetInt32(reader.GetOrdinal("next_priority"))
                        };
                    }

                    return (current, next);
                }

                throw new InvalidOperationException($"No rank found for points: {points}");
            }
            finally
            {
                if (ownsConnection && connection != null)
                {
                    await connection.DisposeAsync();
                }
            }
        }

        private async Task<List<RankLevel>> ReadAsync(
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

                var list = new List<RankLevel>();

                while (await reader.ReadAsync(ct))
                    list.Add(MapRank(reader));

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

        private static RankLevel MapRank(NpgsqlDataReader reader)
        {
            return new RankLevel
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                MinPoints = reader.GetInt32(reader.GetOrdinal("min_points")),
                MaxPoints = reader.IsDBNull(reader.GetOrdinal("max_points")) ? null : reader.GetInt32(reader.GetOrdinal("max_points")),
                Priority = reader.GetInt32(reader.GetOrdinal("priority")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
            };
        }
    }
}