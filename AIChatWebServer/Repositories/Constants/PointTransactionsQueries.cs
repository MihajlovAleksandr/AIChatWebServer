namespace AIChatWebServer.Repositories.Constants
{
    public static class PointTransactionsQueries
    {
        public const string Create = @"
            INSERT INTO point_transactions (
                id,
                user_id,
                amount,
                type,
                reason,
                reference_id,
                created_at
            )
            VALUES (
                gen_random_uuid(),
                @user_id,
                @amount,
                @type,
                @reason,
                @reference_id,
                CURRENT_TIMESTAMP
            )
            RETURNING *;";

        public const string GetByUserId = @"
            SELECT *
            FROM point_transactions
            WHERE user_id = @user_id
            ORDER BY created_at DESC
            LIMIT @limit OFFSET @offset;";

        public const string GetByUserIdAndDateRange = @"
            SELECT *
            FROM point_transactions
            WHERE user_id = @user_id
              AND created_at >= @start_date
              AND created_at <= @end_date
            ORDER BY created_at DESC;";

        public const string GetByReferenceId = @"
            SELECT *
            FROM point_transactions
            WHERE reference_id = @reference_id
            LIMIT 1;";

        public const string GetSumByUserIdAndDateRange = @"
            SELECT COALESCE(SUM(amount), 0)
            FROM point_transactions
            WHERE user_id = @user_id
              AND created_at >= @start_date
              AND created_at <= @end_date;";

        public const string GetByType = @"
            SELECT *
            FROM point_transactions
            WHERE user_id = @user_id
              AND type = @type
            ORDER BY created_at DESC;";

        public const string GetUserBalanceAtDate = @"
            SELECT COALESCE(SUM(amount), 0)
            FROM point_transactions
            WHERE user_id = @user_id
              AND created_at <= @date;";
    }
}