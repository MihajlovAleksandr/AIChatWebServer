namespace AIChatWebServer.Repositories.Constants
{
    public static class AISettingsQueries
    {
        public const string GetByChatId = @"
            SELECT id, chat_id, custom_prompt, model, last_update
            FROM ai_settings
            WHERE chat_id = @chatId
            LIMIT 1;
        ";

        public const string Insert = @"
            INSERT INTO ai_settings (id, chat_id, custom_prompt, model, last_update)
            VALUES (@id, @chatId, @customPrompt, @model, CURRENT_TIMESTAMP);
        ";

        public const string Update = @"
            UPDATE ai_settings
            SET custom_prompt = @customPrompt,
                model = @model,
                last_update = CURRENT_TIMESTAMP
            WHERE chat_id = @chatId;
        ";
    }
}