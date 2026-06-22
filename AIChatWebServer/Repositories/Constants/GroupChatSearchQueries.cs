namespace AIChatWebServer.Repositories.Constants
{
    public static class GroupChatSearchQueries
    {
        public const string Enqueue = @"
            INSERT INTO group_chat_search (
                id,
                chat_id,
                user_id,
                match_predicate,
                slots,
                status,
                created_at
            )
            VALUES (
                @id,
                @chatId,
                @userId,
                @predicate,
                @slots,
                1,
                NOW()
            )
        ";

        public const string LockEntry = @"
            SELECT
                id,
                chat_id,
                user_id,
                match_predicate,
                slots,
                status,
                created_at
            FROM group_chat_search
            WHERE id = @id
            FOR UPDATE;
        ";

        public const string AcquireCandidates = @"
            SELECT
                id,
                chat_id,
                user_id,
                match_predicate,
                slots,
                status,
                created_at
            FROM group_chat_search
            WHERE
                status = 1
                AND user_id <> @userId
                AND slots > 0
            ORDER BY created_at
            FOR UPDATE SKIP LOCKED
            LIMIT @limit;
        ";

        public const string Complete = @"
            UPDATE group_chat_search
            SET
                slots = slots - 1,
                status = CASE
                    WHEN slots - 1 <= 0 THEN 2
                    ELSE status
                END
            WHERE id = ANY(@ids);
        ";

        public const string Cancel = @"
            UPDATE group_chat_search
            SET status = 3
            WHERE id = @id;
        ";

        public const string GetByUser = @"
            SELECT
                id,
                chat_id,
                user_id,
                match_predicate,
                slots,
                status,
                created_at
            FROM group_chat_search
            WHERE
                user_id = @userId
                AND status = 1
            ORDER BY created_at
            LIMIT 1;
        ";
    }
}