namespace AIChatWebServer.Repositories.Constants
{
    public static class PaymentItemQueries
    {
        public const string Insert = @"
            INSERT INTO payment_item (
                id,
                payment_id,
                product_id,
                quantity,
                price
            )
            VALUES (
                @id,
                @payment_id,
                @product_id,
                @quantity,
                @price
            );";

        public const string GetByPayment = @"
            SELECT DISTINCT ON (pi.id)
                pi.id,
                pi.payment_id,
                pi.product_id,
                pi.quantity,
                pi.price,

                p.id AS p_id,
                p.code,
                p.name,
                p.description,
                p.type,
                p.attributes,
                p.is_active,
                p.created_at,

                pp.price AS product_price,
                pp.stripe_price_id,
                r.currency

            FROM payment_item pi

            JOIN product p 
                ON p.id = pi.product_id

            JOIN product_price pp 
                ON pp.product_id = p.id

            JOIN regions r 
                ON r.code = pp.region_code

            WHERE pi.payment_id = @payment_id
              AND pp.region_code IN (@region, 'ZZ')

            ORDER BY 
                pi.id,
                CASE WHEN pp.region_code = @region THEN 0 ELSE 1 END;";

        public const string HasUserPurchasedProduct = @"
            SELECT EXISTS (
                SELECT 1
                FROM payment p
                JOIN payment_item pi ON pi.payment_id = p.id
                WHERE p.user_id = @user_id
                  AND pi.product_id = @product_id
                  AND p.status = @status
            );";
    }
}