namespace AIChatWebServer.Repositories.Constants
{
    public static class UserRankHistoryQueries
    {
        public const string Create = @"
            INSERT INTO user_rank_history (
                id,
                user_id,
                rank_id,
                points_at_moment,
                changed_at
            )
            VALUES (
                gen_random_uuid(),
                @user_id,
                @rank_id,
                @points_at_moment,
                CURRENT_TIMESTAMP
            )
            RETURNING *;";

        public const string GetCurrentRankByUserId = @"
            SELECT urh.*, rl.name as rank_name, rl.min_points, rl.max_points, rl.priority
            FROM user_rank_history urh
            JOIN rank_levels rl ON rl.id = urh.rank_id
            WHERE urh.user_id = @user_id
            ORDER BY urh.changed_at DESC
            LIMIT 1;";

        public const string GetHistoryByUserId = @"
            SELECT urh.*, rl.name as rank_name, rl.min_points, rl.max_points, rl.priority
            FROM user_rank_history urh
            JOIN rank_levels rl ON rl.id = urh.rank_id
            WHERE urh.user_id = @user_id
            ORDER BY urh.changed_at DESC
            LIMIT @limit OFFSET @offset;";

        public const string GetUsersWhoChangedRankBetweenDates = @"
            SELECT DISTINCT urh.user_id
            FROM user_rank_history urh
            WHERE urh.changed_at >= @start_date
              AND urh.changed_at <= @end_date;";

        public const string GetLastRankChangeByUser = @"
            SELECT *
            FROM user_rank_history
            WHERE user_id = @user_id
            ORDER BY changed_at DESC
            LIMIT 1;";

        public const string GetRankHistoryByDateRange = @"
            SELECT urh.*, rl.name as rank_name
            FROM user_rank_history urh
            JOIN rank_levels rl ON rl.id = urh.rank_id
            WHERE urh.user_id = @user_id
              AND urh.changed_at >= @start_date
              AND urh.changed_at <= @end_date
            ORDER BY urh.changed_at ASC;";

        public const string GetUsersInRank = @"
            SELECT DISTINCT ON (urh.user_id) urh.user_id
            FROM user_rank_history urh
            WHERE urh.rank_id = @rank_id
            ORDER BY urh.user_id, urh.changed_at DESC;";
    }
}