namespace AIChatWebServer.Repositories.Constants
{
    public static class ThemeQueries
    {
        public const string GetById = @"
            SELECT 
                id,
                user_id,
                name,
                type,
                config_json,
                created_at,
                updated_at
            FROM themes
            WHERE id = @id;";

        public const string GetByName = @"
            SELECT 
                id,
                user_id,
                name,
                type,
                config_json,
                created_at,
                updated_at
            FROM themes
            WHERE user_id = @userId AND name = @name;";

        public const string GetByUserId = @"
            SELECT 
                id,
                user_id,
                name,
                type,
                config_json,
                created_at,
                updated_at
            FROM themes
            WHERE user_id = @user_id OR (user_id IS NULL AND @user_id IS NULL)
            ORDER BY created_at DESC;";

        public const string GetByType = @"
            SELECT 
                id,
                user_id,
                name,
                type,
                config_json,
                created_at,
                updated_at
            FROM themes
            WHERE type = @type
            ORDER BY created_at DESC;";

        public const string GetAll = @"
            SELECT 
                id,
                user_id,
                name,
                type,
                config_json,
                created_at,
                updated_at
            FROM themes
            ORDER BY created_at DESC;";

        public const string Create = @"
            INSERT INTO themes (
                id,
                user_id,
                name,
                type,
                config_json,
                created_at,
                updated_at
            ) VALUES (
                @id,
                @user_id,
                @name,
                @type,
                @config_json,
                CURRENT_TIMESTAMP,
                CURRENT_TIMESTAMP
            );";

        public const string Update = @"
            UPDATE themes
            SET 
                name = @name,
                type = @type,
                config_json = @config_json,
                updated_at = @updated_at
            WHERE id = @id;";

        public const string Delete = @"
            DELETE FROM themes
            WHERE id = @id;";

        public const string GetSelectedThemeByConnectionId = @"
            SELECT 
                t.id,
                t.user_id,
                t.name,
                t.type,
                t.config_json,
                t.created_at,
                t.updated_at
            FROM selected_themes st
            INNER JOIN themes t ON st.theme_id = t.id
            WHERE st.connection_id = @connection_id;";

        public const string UpsertSelectedTheme = @"
            INSERT INTO selected_themes (
                id,
                connection_id,
                theme_id,
                selected_at
            ) VALUES (
                gen_random_uuid(),
                @connection_id,
                @theme_id,
                CURRENT_TIMESTAMP
            )
            ON CONFLICT (connection_id) 
            DO UPDATE SET
                theme_id = EXCLUDED.theme_id,
                selected_at = CURRENT_TIMESTAMP
            RETURNING id;";

        public const string GetThemeUsageCount = @"
            SELECT COUNT(1) 
            FROM selected_themes 
            WHERE theme_id = @theme_id;";

        public const string DeleteSelectedTheme = @"
            DELETE FROM selected_themes
            WHERE connection_id = @connection_id;";
    }
}