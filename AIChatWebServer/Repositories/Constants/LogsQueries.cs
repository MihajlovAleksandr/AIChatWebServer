namespace AIChatWebServer.Repositories.Constants
{
    namespace AIChatWebServer.Repositories.Constants
    {
        public static class LogsQueries
        {
            public const string GetAll = @"
            SELECT id, timestamp, level, message, source
            FROM logs
            ORDER BY timestamp DESC;
        ";

            public const string GetById = @"
            SELECT id, timestamp, level, message, source
            FROM logs
            WHERE id = @id
            LIMIT 1;
        ";

            public const string GetByLevel = @"
            SELECT id, timestamp, level, message, source
            FROM logs
            WHERE level = @level
            ORDER BY timestamp DESC;
        ";

            public const string GetBySource = @"
            SELECT id, timestamp, level, message, source
            FROM logs
            WHERE source = @source
            ORDER BY timestamp DESC;
        ";

            public const string GetByDateRange = @"
            SELECT id, timestamp, level, message, source
            FROM logs
            WHERE timestamp BETWEEN @startDate AND @endDate
            ORDER BY timestamp DESC;
        ";

            public const string Insert = @"
            INSERT INTO logs (id, timestamp, level, message, source)
            VALUES (@id, @timestamp, @level, @message, @source);
        ";

            public const string DeleteOldLogs = @"
            DELETE FROM logs
            WHERE timestamp < @cutoffDate;
        ";

            public const string DeleteById = @"
            DELETE FROM logs
            WHERE id = @id;
        ";
        }
    }
}
