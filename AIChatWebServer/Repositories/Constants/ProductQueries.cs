namespace AIChatWebServer.Repositories.Constants
{
    public static class ProductQueries
    {
        public const string GetById = @"
            SELECT 
                p.*,
                pp.price,
                pp.stripe_price_id,
                r.currency
            FROM product p
            JOIN product_price pp 
                ON pp.product_id = p.id
            JOIN regions r 
                ON r.code = pp.region_code
            WHERE p.id = @id
              AND pp.region_code IN (@region, 'ZZ')
            ORDER BY 
                CASE WHEN pp.region_code = @region THEN 0 ELSE 1 END
            LIMIT 1;";

        public const string GetByCode = @"
            SELECT 
                p.*,
                pp.price,
                pp.stripe_price_id,
                r.currency
            FROM product p
            JOIN product_price pp 
                ON pp.product_id = p.id
            JOIN regions r 
                ON r.code = pp.region_code
            WHERE p.code = @code
              AND pp.region_code IN (@region, 'ZZ')
            ORDER BY 
                CASE WHEN pp.region_code = @region THEN 0 ELSE 1 END
            LIMIT 1;";

        public const string GetByStripePriceId = @"
            SELECT 
                p.*,
                pp.price,
                pp.stripe_price_id,
                r.currency
            FROM product p
            JOIN product_price pp 
                ON pp.product_id = p.id
            JOIN regions r 
                ON r.code = pp.region_code
            WHERE pp.stripe_price_id = @stripe_price_id
              AND pp.region_code IN (@region, 'ZZ')
            ORDER BY 
                CASE WHEN pp.region_code = @region THEN 0 ELSE 1 END
            LIMIT 1;";

        public const string GetActive = @"
            SELECT DISTINCT ON (p.id)
                p.*,
                pp.price,
                pp.stripe_price_id,
                r.currency
            FROM product p
            JOIN product_price pp 
                ON pp.product_id = p.id
            JOIN regions r 
                ON r.code = pp.region_code
            WHERE p.is_active = TRUE
              AND pp.region_code IN (@region, 'ZZ')
            ORDER BY 
                p.id,
                CASE WHEN pp.region_code = @region THEN 0 ELSE 1 END,
                p.created_at DESC;";

        public const string GetByType = @"
            SELECT DISTINCT ON (p.id)
                p.*,
                pp.price,
                pp.stripe_price_id,
                r.currency
            FROM product p
            JOIN product_price pp 
                ON pp.product_id = p.id
            JOIN regions r 
                ON r.code = pp.region_code
            WHERE p.type = @type
              AND (@only_active = FALSE OR p.is_active = TRUE)
              AND pp.region_code IN (@region, 'ZZ')
            ORDER BY 
                p.id,
                CASE WHEN pp.region_code = @region THEN 0 ELSE 1 END,
                p.created_at DESC;";
    }
}