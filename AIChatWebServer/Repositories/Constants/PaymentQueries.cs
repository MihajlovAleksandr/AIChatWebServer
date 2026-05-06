namespace AIChatWebServer.Repositories.Constants
{
    public static class PaymentQueries
    {
        public const string Insert = @"
            INSERT INTO payment (
                id,
                transaction_id,
                user_id,
                amount,
                currency,
                status
            )
            VALUES (
                @id,
                @transaction_id,
                @userId,
                @amount,
                @currency,
                @status
            );";

        public const string GetById = @"
            SELECT *
            FROM payment
            WHERE id = @id;";

        public const string GetByUser = @"
            SELECT *
            FROM payment
            WHERE user_id = @user_id
            ORDER BY created_at DESC;";

        public const string UpdateStatus = @"
            UPDATE payment
            SET 
                status = @status,
                transaction_id = @transaction_id
            WHERE id = @id;";

        public const string ExistsByTransactionId = @"
            SELECT EXISTS (
                SELECT 1
                FROM payment
                WHERE transaction_id = @transaction_id
            );";
    }
}