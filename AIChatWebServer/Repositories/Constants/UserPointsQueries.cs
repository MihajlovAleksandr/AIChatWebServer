namespace AIChatWebServer.Repositories.Constants
{
    public static class UserPointsQueries
    {
        public const string FindOrCreateByUserId = @"
            INSERT INTO user_points (user_id, total_points, last_updated)
            VALUES (@user_id, 0, CURRENT_TIMESTAMP)
            ON CONFLICT (user_id) DO NOTHING
            RETURNING *;";

        public const string FindOrCreateByUserIdSelect = @"
            SELECT *
            FROM user_points
            WHERE user_id = @user_id;";

        public const string GetByUserId = @"
            SELECT *
            FROM user_points
            WHERE user_id = @user_id;";

        public const string UpdateTotalPoints = @"
            UPDATE user_points
            SET total_points = @total_points,
                last_updated = CURRENT_TIMESTAMP
            WHERE user_id = @user_id
            RETURNING *;";

        public const string IncrementPoints = @"
            UPDATE user_points
            SET total_points = total_points + @amount,
                last_updated = CURRENT_TIMESTAMP
            WHERE user_id = @user_id
            RETURNING *;";

        public const string DecrementPoints = @"
            UPDATE user_points
            SET total_points = GREATEST(total_points - @amount, 0),
                last_updated = CURRENT_TIMESTAMP
            WHERE user_id = @user_id
            RETURNING *;";

        public const string GetByPointsRange = @"
            SELECT *
            FROM user_points
            WHERE total_points BETWEEN @min_points AND @max_points
            ORDER BY total_points DESC;";

        public const string GetLeaderboard = @"
            SELECT up.*
            FROM user_points up
            ORDER BY up.total_points DESC
            LIMIT @limit OFFSET @offset;";

        public const string GetLeaderboardTotalCount = @"
            SELECT COUNT(*) FROM user_points";
    }
}