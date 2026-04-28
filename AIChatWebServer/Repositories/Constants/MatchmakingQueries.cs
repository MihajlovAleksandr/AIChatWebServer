namespace AIChatWebServer.Repositories.Constants
{
    public static class MatchmakingQueries
    {
        public const string Enqueue = @"
            INSERT INTO matchmaking_queue (
                id,
                user_id,
                chat_type,
                chat_name,
                match_predicate,
                status,
                created_at,
                expires_at
            )
            VALUES (
                @id,
                @userId,
                @chatType,
                @chatName,
                @predicate,
                1,
                NOW(),
                @expiresAt
            );
        ";

        public const string LockEntry = @"
            SELECT
                id,
                user_id,
                chat_type,
                chat_name,
                match_predicate,
                status,
                created_at,
                expires_at
            FROM matchmaking_queue
            WHERE id = @id
            FOR UPDATE;
        ";

        public const string AcquireCandidates = @"
            SELECT
                id,
                user_id,
                chat_type,
                chat_name,
                match_predicate,
                status,
                created_at,
                expires_at
            FROM matchmaking_queue
            WHERE
                chat_type = @chatType
                AND status = 1
                AND user_id <> @userId
            ORDER BY created_at
            FOR UPDATE SKIP LOCKED
            LIMIT @limit;
        ";

        public const string Complete = @"
            UPDATE matchmaking_queue
            SET status = 2
            WHERE id = ANY(@ids);
        ";

        public const string Cancel = @"
            UPDATE matchmaking_queue
            SET status = 3
            WHERE id = @id;
        ";

        public const string GetChatByUser = @"
            SELECT
                id,
                user_id,
                chat_type,
                chat_name,
                match_predicate,
                status,
                created_at,
                expires_at
            FROM matchmaking_queue
            WHERE
                user_id = @userId
                AND status = 1
                AND chat_type <> 'Group'
            ORDER BY created_at
            LIMIT 1;
        ";

        public const string GetGroupByUser = @"
            SELECT
                id,
                user_id,
                chat_type,
                chat_name,
                match_predicate,
                status,
                created_at,
                expires_at
            FROM matchmaking_queue
            WHERE
                user_id = @userId
                AND status = 1
                AND chat_type = 'Group'
            ORDER BY created_at
            LIMIT 1;
        ";

        public const string Expire = @"
            UPDATE matchmaking_queue
            SET status = 4
            WHERE
                status = 1
                AND expires_at IS NOT NULL
                AND expires_at < NOW();
        ";
    }
}