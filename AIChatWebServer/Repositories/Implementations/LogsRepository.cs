using AIChatWebServer.Models.LogModel;
using AIChatWebServer.Repositories.Constants.AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;
using NpgsqlTypes;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class LogsRepository(IConfiguration configuration) : BaseRepository(configuration), ILogsRepository
    {
        public async Task<IEnumerable<LogModel>> GetAll(
            CancellationToken cancellationToken = default)
        {
            var logs = new List<LogModel>();

            await using var conn = await GetConnectionAsync(cancellationToken);
            await using var cmd = new NpgsqlCommand(LogsQueries.GetAll, conn);

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                logs.Add(MapToLogModel(reader));
            }

            return logs;
        }

        public async Task<LogModel?> GetById(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            await using var conn = await GetConnectionAsync(cancellationToken);
            await using var cmd = new NpgsqlCommand(LogsQueries.GetById, conn);

            cmd.Parameters.AddWithValue("@id", id);

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            if (await reader.ReadAsync(cancellationToken))
            {
                return MapToLogModel(reader);
            }

            return null;
        }

        public async Task<IEnumerable<LogModel>> GetByLevel(
            string level,
            CancellationToken cancellationToken = default)
        {
            var logs = new List<LogModel>();

            await using var conn = await GetConnectionAsync(cancellationToken);
            await using var cmd = new NpgsqlCommand(LogsQueries.GetByLevel, conn);

            cmd.Parameters.AddWithValue("@level", level);

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                logs.Add(MapToLogModel(reader));
            }

            return logs;
        }

        public async Task<IEnumerable<LogModel>> GetBySource(
            string source,
            CancellationToken cancellationToken = default)
        {
            var logs = new List<LogModel>();

            await using var conn = await GetConnectionAsync(cancellationToken);
            await using var cmd = new NpgsqlCommand(LogsQueries.GetBySource, conn);

            cmd.Parameters.AddWithValue("@source", source);

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                logs.Add(MapToLogModel(reader));
            }

            return logs;
        }

        public async Task<IEnumerable<LogModel>> GetByDateRange(
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default)
        {
            var logs = new List<LogModel>();

            await using var conn = await GetConnectionAsync(cancellationToken);
            await using var cmd = new NpgsqlCommand(LogsQueries.GetByDateRange, conn);

            cmd.Parameters.AddWithValue("@startDate", startDate);
            cmd.Parameters.AddWithValue("@endDate", endDate);

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                logs.Add(MapToLogModel(reader));
            }

            return logs;
        }

        public async Task<LogModel> Add(
            string level,
            string message,
            string? source = null,
            CancellationToken cancellationToken = default)
        {
            await using var conn = await GetConnectionAsync(cancellationToken);

            var id = Guid.NewGuid();
            var timestamp = DateTime.UtcNow;

            await using var cmd = new NpgsqlCommand(LogsQueries.Insert, conn);

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@timestamp", timestamp);
            cmd.Parameters.AddWithValue("@level", level);
            cmd.Parameters.AddWithValue("@message", NpgsqlDbType.Text, message);

            if (source is null)
                cmd.Parameters.AddWithValue("@source", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@source", source);

            await cmd.ExecuteNonQueryAsync(cancellationToken);

            return new LogModel(id, timestamp, level, message, source);
        }

        public async Task<int> DeleteOldLogs(
            DateTime cutoffDate,
            CancellationToken cancellationToken = default)
        {
            await using var conn = await GetConnectionAsync(cancellationToken);
            await using var cmd = new NpgsqlCommand(LogsQueries.DeleteOldLogs, conn);

            cmd.Parameters.AddWithValue("@cutoffDate", cutoffDate);

            var rows = await cmd.ExecuteNonQueryAsync(cancellationToken);

            return rows;
        }

        public async Task<bool> DeleteById(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            await using var conn = await GetConnectionAsync(cancellationToken);
            await using var cmd = new NpgsqlCommand(LogsQueries.DeleteById, conn);

            cmd.Parameters.AddWithValue("@id", id);

            var rows = await cmd.ExecuteNonQueryAsync(cancellationToken);

            return rows > 0;
        }

        private static LogModel MapToLogModel(NpgsqlDataReader reader)
        {
            return new LogModel(
                reader.GetGuid(reader.GetOrdinal("id")),
                reader.GetDateTime(reader.GetOrdinal("timestamp")),
                reader.GetString(reader.GetOrdinal("level")),
                reader.GetString(reader.GetOrdinal("message")),
                reader.IsDBNull(reader.GetOrdinal("source"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("source"))
            );
        }
    }
}