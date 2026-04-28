namespace AIChatWebServer.Repositories.Constants
{
    public static class AIMessageQueries
    {
        public const string InsertMessage = @"
            INSERT INTO ai_messages (id, chat_id, text, role, type)
            VALUES (@id, @chatId, @text, @role, @type);";

        public const string GetMessageById = @"
            SELECT id, chat_id, text, role, type
            FROM ai_messages
            WHERE id = @id;";

        public const string GetMessagesByChatId = @"
            SELECT id, chat_id, text, role, type
            FROM ai_messages
            WHERE chat_id = @chatId
            ORDER BY created_at;";

        public const string DeleteMessage = @"
            DELETE FROM ai_messages
            WHERE id = @id;";

        public const string UseTokens = @"
            INSERT INTO chat_token_usage (chat_id, tokens_count, operation, model, date)
            VALUES (@chatId, @tokensCount, @operation, @model, @date)";
    }
}