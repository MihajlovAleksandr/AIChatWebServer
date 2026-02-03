namespace AIChatWebServer.Repositories.Constants
{
    public static class VerificationCodeQueries
    {
        public const string Upsert = """
            INSERT INTO verification_codes (user_id, type, code_hash, expires_at)
            VALUES (@UserId, @Type, @CodeHash, @ExpiresAt)
            ON CONFLICT (user_id, type)
            DO UPDATE SET
                code_hash = EXCLUDED.code_hash,
                expires_at = EXCLUDED.expires_at,
                attempts = 0,
                created_at = CURRENT_TIMESTAMP;
            """;

        public const string Get = """
            SELECT id, user_id, type, code_hash, attempts, expires_at, created_at
            FROM verification_codes
            WHERE user_id = @UserId AND type = @Type
            LIMIT 1;
            """;

        public const string IncrementAttempts = """
            UPDATE verification_codes
            SET attempts = attempts + 1
            WHERE id = @Id;
            """;

        public const string Delete = """
            DELETE FROM verification_codes
            WHERE id = @Id;
            """;
    }
}
