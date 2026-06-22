namespace AIChatWebServer.Repositories.Constants
{
    public static class RankLevelsQueries
    {
        public const string FindRankByPoints = @"
            SELECT *
            FROM rank_levels
            WHERE @points >= min_points
              AND (max_points IS NULL OR @points <= max_points)
            LIMIT 1;";

        public const string FindRankByPriority = @"
            SELECT *
            FROM rank_levels
            WHERE priority = @priority
            LIMIT 1;";

        public const string FindAllRanksSortedByPriority = @"
            SELECT *
            FROM rank_levels
            ORDER BY priority ASC;";

        public const string FindRankByMinPoints = @"
            SELECT *
            FROM rank_levels
            WHERE min_points = @min_points
            LIMIT 1;";

        public const string GetNextRank = @"
            SELECT *
            FROM rank_levels
            WHERE priority > (SELECT priority FROM rank_levels WHERE id = @rank_id)
            ORDER BY priority ASC
            LIMIT 1;";

        public const string GetPreviousRank = @"
            SELECT *
            FROM rank_levels
            WHERE priority < (SELECT priority FROM rank_levels WHERE id = @rank_id)
            ORDER BY priority DESC
            LIMIT 1;";

        public const string GetRankByMinPointsWithNext = @"
            SELECT 
                current.*,
                next.id as next_id,
                next.name as next_name,
                next.min_points as next_min_points,
                next.max_points as next_max_points,
                next.priority as next_priority
            FROM rank_levels current
            LEFT JOIN rank_levels next ON next.min_points = (
                SELECT MIN(min_points) 
                FROM rank_levels 
                WHERE min_points > current.min_points
            )
            WHERE current.min_points <= @points 
              AND (current.max_points IS NULL OR @points <= current.max_points)
            LIMIT 1;";
    }
}