namespace AIChatWebServer.Repositories.Constants
{
    public static class UserPremiumQueries
    {
        public const string Create = @"
            INSERT INTO users_premium (
                id,
                user_id,
                payment_item_id,
                start_at,
                end_at,
                is_auto_renew,
                subscription_id
            )
            VALUES (
                @id,
                @user_id,
                @payment_item_id,
                @start_at,
                @end_at,
                @is_auto_renew,
                @subscription_id
            );";

        public const string GetActive = @"
            SELECT *
            FROM users_premium
            WHERE user_id = @user_id
              AND start_at <= NOW()
              AND end_at > NOW()
            ORDER BY end_at DESC
            LIMIT 1;";

        public const string GetLast = @"
            SELECT *
            FROM users_premium
            WHERE user_id = @user_id
            ORDER BY end_at DESC
            LIMIT 1;";

        public const string GetByUser = @"
            SELECT *
            FROM users_premium
            WHERE user_id = @user_id
            ORDER BY start_at DESC;";

        public const string GetById = @"
            SELECT *
            FROM users_premium
            WHERE id = @id;";

        public const string GetAutoRenew = @"
            SELECT *
            FROM users_premium
            WHERE user_id = @userId
            AND is_auto_renew = true
            LIMIT 1
        ";

        public const string CancelAutoRenew = @"
            UPDATE users_premium
            SET is_auto_renew = false
            WHERE subscription_id = @subscriptionId;";

        public const string GetUserIdBySubscriptionId = @"
            SELECT user_id
            FROM users_premium
            WHERE subscription_id = @subscription_id
            ORDER BY end_at DESC
            LIMIT 1;";

        public const string GetFirstBySubscriptionId = @"
            SELECT *
            FROM users_premium
            WHERE subscription_id = @subscription_id
            ORDER BY start_at ASC
            LIMIT 1;";
    }
}