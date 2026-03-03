namespace AIChatWebServer.Repositories.Constants
{
    public static class ChatQueries
    {
        public const string GetChatById = @"
            SELECT 
                c.id,
                c.creation_time,
                c.end_time,
                c.type,

                cs.allow_add_by_link,
                cs.allow_search_join,
                cs.call_enabled,
                cs.video_enabled,

                uc.id,
                uc.user_id,
                uc.name,
                uc.join_time,

                ucs.role,
                ucs.can_add_users_by_search,
                ucs.can_add_user_by_link,
                ucs.can_remove_users,
                ucs.can_change_user_settings,
                ucs.can_change_chat_settings,
                ucs.can_start_calls

            FROM chats c

            LEFT JOIN chat_settings cs
                ON cs.chat_id = c.id

            LEFT JOIN users_chats uc 
                ON uc.chat_id = c.id
                AND uc.deleted_status = FALSE

            LEFT JOIN user_chat_settings ucs
                ON ucs.user_chat_id = uc.id

            WHERE c.id = @chatId
              AND c.deleted_status = FALSE;";

        public const string CreateChat = @"
            INSERT INTO chats (id, type)
            VALUES (@id, @type);";

        public const string CreateChatSettings = @"
            INSERT INTO chat_settings (
                chat_id,
                allow_add_by_link,
                allow_search_join,
                call_enabled,
                video_enabled)
            VALUES (
                @chatId,
                TRUE,
                TRUE,
                TRUE,
                TRUE);";

        public const string AddUserToChat = @"
            INSERT INTO users_chats (id, user_id, chat_id, name)
            VALUES (@id, @userId, @chatId, @name);";

        public const string AddUserSettings = @"
            INSERT INTO user_chat_settings (
                user_chat_id,
                role,
                can_add_users_by_search,
                can_add_user_by_link,
                can_remove_users,
                can_change_user_settings,
                can_change_chat_settings,
                can_start_calls)
            VALUES (
                @usersChatId,
                @role,
                @canAddUsersBySearch,
                @canAddUserByLink,
                @canRemoveUsers,
                @canChangeUserSettings,
                @canChangeChatSettings,
                @canStartCalls);";

        public const string UpdateChatName = @"
            UPDATE users_chats
            SET name = @name,
                last_update = CURRENT_TIMESTAMP
            WHERE chat_id = @chatId
              AND user_id = @userId
              AND deleted_status = FALSE;";

        public const string DeleteUserFromChat = @"
            UPDATE users_chats
            SET deleted_status = TRUE,
                last_update = CURRENT_TIMESTAMP
            WHERE chat_id = @chatId
              AND user_id = @userId;";

        public const string EndChat = @"
            UPDATE chats
            SET end_time = CURRENT_TIMESTAMP
            WHERE id = @chatId;";

        public const string UpdateChatSettings = @"
            UPDATE chat_settings
            SET allow_add_by_link = @allowAddByLink,
                allow_search_join = @allowSearchJoin,
                call_enabled = @callEnabled,
                video_enabled = @videoEnabled
            WHERE chat_id = @chatId;";

        public const string UpdateUserSettings = @"
            UPDATE user_chat_settings ucs
            SET role = @role,
                can_add_users_by_search = @canAddUsersBySearch,
                can_add_user_by_link = @canAddUserByLink,
                can_remove_users = @canRemoveUsers,
                can_change_user_settings = @canChangeUserSettings,
                can_change_chat_settings = @canChangeChatSettings,
                can_start_calls = @canStartCalls
            FROM users_chats uc
            WHERE ucs.user_chat_id = uc.id
              AND uc.chat_id = @chatId
              AND uc.user_id = @userId
              AND uc.deleted_status = FALSE;";
    }
}