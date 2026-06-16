namespace AIChatWebServer.Repositories.Constants
{
    public static class PaymentQueries
    {
        public const string Insert = @"
            INSERT INTO payment (
                id,
                transaction_id,
                stripe_charge_id,
                user_id,
                amount,
                currency,
                stripe_invoice_url,
                status
            )
            VALUES (
                @id,
                @transaction_id,
                @stripe_charge_id,
                @userId,
                @amount,
                @currency,
                @stripe_invoice_url,
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

        public const string GetByStripeChargeId = @"
            SELECT *
            FROM payment
            WHERE stripe_charge_id = @stripe_charge_id
            LIMIT 1;";

        public const string UpdateStatus = @"
            UPDATE payment
            SET 
                status = @status,
                transaction_id = @transaction_id,
                stripe_charge_id = @stripe_charge_id,
                stripe_invoice_url = @stripe_invoice_url
            WHERE id = @id;";

        public const string UpdateReceiptUrl = @"
            UPDATE payment
            SET stripe_invoice_url = @stripe_invoice_url
            WHERE stripe_charge_id = @stripe_charge_id;";

        public const string GetByTransactionId = @"
            SELECT *
            FROM payment
            WHERE transaction_id = @transaction_id
            LIMIT 1;";

        public const string ExistsByTransactionId = @"
            SELECT EXISTS (
                SELECT 1
                FROM payment
                WHERE transaction_id = @transaction_id
            );";

        public const string Update = @"
            UPDATE payment
            SET
                transaction_id = @transaction_id,
                stripe_charge_id = @stripe_charge_id,
                stripe_invoice_url = @stripe_invoice_url,
                amount = @amount,
                currency = @currency,
                status = @status
            WHERE id = @id;";

        public const string GetByPremiumId = @"
            SELECT p.*
            FROM payment p
            INNER JOIN payment_item pi
                ON pi.payment_id = p.id
            INNER JOIN users_premium up
                ON up.payment_item_id = pi.id
            WHERE up.id = @premium_id
            LIMIT 1;";

        public const string ExpirePendingPayments = @"
            UPDATE payment
            SET status = @expired_status
            WHERE status = @pending_status
              AND created_at <= @expired_before;";
    }
}