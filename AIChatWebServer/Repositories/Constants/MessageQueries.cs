namespace AIChatWebServer.Repositories.Constants;

public static class MessageQueries
{
    public const string Create = @"
        INSERT INTO messages (id, chat_id, user_id, text)
        VALUES (@id, @chatId, @userId, @text);
    ";

    public const string InsertMessageFile = @"
        INSERT INTO message_files (message_id, file_id)
        VALUES (@messageId, @fileId);
    ";

    public const string InsertReply = @"
        INSERT INTO message_replies (message_id, reply_message_id, start_index_quote, end_index_quote)
        VALUES (@messageId, @replyMessageId, @startIndexQuote, @endIndexQuote);
    ";

    public const string GetById = @"
        SELECT * FROM messages
        WHERE id = @id AND deleted_status = FALSE;
    ";

    public const string GetByChat = @"
        SELECT * FROM messages
        WHERE chat_id = @chatId AND deleted_status = FALSE
        ORDER BY time;
    ";

    public const string UpdateText = @"
        UPDATE messages
        SET text = @text,
            last_update = @updatedAt
        WHERE id = @id;
    ";

    public const string UpsertStatus = @"
        INSERT INTO message_statuses (message_id, user_id, status, last_update)
        VALUES (@messageId, @userId, @status, @updatedAt)
        ON CONFLICT (message_id, user_id)
        DO UPDATE SET
            status = EXCLUDED.status,
            last_update = EXCLUDED.last_update;
    ";

    public const string DeleteMessage = @"
        UPDATE messages
        SET deleted_status = TRUE
        WHERE id = @id;
    ";

    public const string DeleteMessageFile = @"
        DELETE FROM message_files
        WHERE message_id = @messageId AND file_id = @fileId;
    ";

    public const string GetStatuses = @"
        SELECT message_id, user_id, status
        FROM message_statuses
        WHERE message_id = ANY(@ids);
    ";

    public const string GetReplies = @"
        SELECT message_id, reply_message_id, start_index_quote, end_index_quote
        FROM message_replies
        WHERE message_id = ANY(@ids);
    ";

    public const string GetMessageFiles = @"
        SELECT mf.message_id, f.*
        FROM message_files mf
        JOIN files f ON f.id = mf.file_id
        WHERE mf.message_id = ANY(@ids)
          AND f.deleted_status = FALSE;
    ";

    public const string GetChanges = @"
        WITH msg AS (
            SELECT 
                m.*,
                BOOL_OR(ms.last_update > @since) AS has_status_update
            FROM messages m
            LEFT JOIN message_statuses ms 
                ON ms.message_id = m.id
            WHERE m.chat_id = ANY(@chatIds)
            GROUP BY m.id
        )
        
        SELECT
            CASE
                WHEN m.deleted_status = TRUE 
                     AND m.last_update > @since
                THEN 'deleted'
        
                WHEN m.time > @since 
                     AND m.deleted_status = FALSE
                THEN 'created'
        
                WHEN m.deleted_status = FALSE
                     AND (
                         (m.last_update > @since AND m.time <= @since)
                         OR m.has_status_update
                     )
                THEN 'updated'
        
                ELSE NULL
            END AS type,
        
            m.*
        
        FROM msg m
        WHERE
            (
                m.deleted_status = TRUE 
                AND m.last_update > @since
            )
            OR
            (
                m.deleted_status = FALSE
                AND (
                    m.time > @since
                    OR m.last_update > @since
                    OR m.has_status_update
                )
            );
    ";
}