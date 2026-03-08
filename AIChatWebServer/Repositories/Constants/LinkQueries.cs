namespace AIChatWebServer.Repositories.Constants
{
    public static class LinkQueries
    {
        public const string CreateLink = @"
            INSERT INTO links (
                id,
                token_hash,
                type,
                payload,
                created_by,
                expires_at,
                max_uses,
                current_uses,
                revoked,
                created_at
            )
            VALUES (
                @id,
                @tokenHash,
                @type,
                @payload::jsonb,
                @createdBy,
                @expiresAt,
                @maxUses,
                0,
                false,
                NOW()
            );
        ";

        public const string GetByTokenHash = @"
            SELECT
                id,
                token_hash,
                type,
                payload::text,
                created_by,
                expires_at,
                max_uses,
                current_uses,
                revoked,
                created_at
            FROM links
            WHERE token_hash = @tokenHash;
        ";

        public const string GetById = @"
            SELECT
                id,
                token_hash,
                type,
                payload::text,
                created_by,
                expires_at,
                max_uses,
                current_uses,
                revoked,
                created_at
            FROM links
            WHERE id = @id;
        ";

        public const string IncrementUsageIfAllowed = @"
            UPDATE links
            SET current_uses = current_uses + 1
            WHERE
                id = @id
                AND revoked = false
                AND (expires_at IS NULL OR expires_at > NOW())
                AND current_uses < max_uses;
        ";

        public const string Revoke = @"
            UPDATE links
            SET revoked = true
            WHERE id = @id;
        ";

        public const string Delete = @"
            DELETE FROM links
            WHERE id = @id;
        ";

        public const string GetActiveByCreator = @"
            SELECT
                id,
                token_hash,
                type,
                payload::text,
                created_by,
                expires_at,
                max_uses,
                current_uses,
                revoked,
                created_at
            FROM links
            WHERE
                created_by = @createdBy
                AND revoked = false
                AND (expires_at IS NULL OR expires_at > NOW());
        ";
    }
}