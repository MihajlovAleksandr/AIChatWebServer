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
                cs.message_files_enabled,
                cs.message_images_enabled,
                cs.voice_message_enabled,
                cs.video_message_enabled,

                uc.id AS user_chat_id,
                uc.user_id,
                uc.name,
                uc.join_time,

                ucs.role,
                ucs.can_add_users_by_search,
                ucs.can_add_user_by_link,
                ucs.can_remove_users,
                ucs.can_change_user_settings,
                ucs.can_change_chat_settings,
                ucs.can_start_calls,
                ucs.messages_enabled,
                ucs.message_files_enabled,
                ucs.message_images_enabled,
                ucs.voice_message_enabled,
                ucs.video_message_enabled,
                ucs.edit_messages_enabled,
                ucs.delete_own_messages_enabled,
                ucs.delete_other_messages_enabled

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
                video_enabled,
                message_files_enabled,
                message_images_enabled,
                voice_message_enabled,
                video_message_enabled)
            VALUES (
                @chatId,
                @allowAddByLink,
                @allowSearchJoin,
                @callEnabled,
                @videoEnabled,
                @messageFilesEnabled,
                @messageImagesEnabled,
                @voiceMessageEnabled,
                @videoMessageEnabled);";

        public const string AddUserToChat = @"
            INSERT INTO users_chats (id, user_id, chat_id, name)
            VALUES (@id, @userId, @chatId, @name)

            ON CONFLICT (chat_id, user_id)
            DO UPDATE SET
                deleted_status = FALSE,
                name = EXCLUDED.name,
                join_time = CURRENT_TIMESTAMP,
                last_update = CURRENT_TIMESTAMP
            
            RETURNING id;";

        public const string UpsertUserSettings = @"
            INSERT INTO user_chat_settings (
                user_chat_id,
                role,
                can_add_users_by_search,
                can_add_user_by_link,
                can_remove_users,
                can_change_user_settings,
                can_change_chat_settings,
                can_start_calls,
                messages_enabled,
                message_files_enabled,
                message_images_enabled,
                voice_message_enabled,
                video_message_enabled,
                edit_messages_enabled,
                delete_own_messages_enabled,
                delete_other_messages_enabled)
            VALUES (
                @usersChatId,
                @role,
                @canAddUsersBySearch,
                @canAddUserByLink,
                @canRemoveUsers,
                @canChangeUserSettings,
                @canChangeChatSettings,
                @canStartCalls,
                @messagesEnabled,
                @messageFilesEnabled,
                @messageImagesEnabled,
                @voiceMessageEnabled,
                @videoMessageEnabled,
                @editMessagesEnabled,
                @deleteOwnMessagesEnabled,
                @deleteOtherMessagesEnabled)
            
            ON CONFLICT (user_chat_id)
            DO UPDATE SET
                role = EXCLUDED.role,
                can_add_users_by_search = EXCLUDED.can_add_users_by_search,
                can_add_user_by_link = EXCLUDED.can_add_user_by_link,
                can_remove_users = EXCLUDED.can_remove_users,
                can_change_user_settings = EXCLUDED.can_change_user_settings,
                can_change_chat_settings = EXCLUDED.can_change_chat_settings,
                can_start_calls = EXCLUDED.can_start_calls,
                messages_enabled = EXCLUDED.messages_enabled,
                message_files_enabled = EXCLUDED.message_files_enabled,
                message_images_enabled = EXCLUDED.message_images_enabled,
                voice_message_enabled = EXCLUDED.voice_message_enabled,
                video_message_enabled = EXCLUDED.video_message_enabled,
                edit_messages_enabled = EXCLUDED.edit_messages_enabled,
                delete_own_messages_enabled = EXCLUDED.delete_own_messages_enabled,
                delete_other_messages_enabled = EXCLUDED.delete_other_messages_enabled;";

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
            SET end_time = @endTime
            WHERE id = @chatId;";

        public const string UpdateChatSettings = @"
            UPDATE chat_settings
            SET allow_add_by_link = @allowAddByLink,
                allow_search_join = @allowSearchJoin,
                call_enabled = @callEnabled,
                video_enabled = @videoEnabled,
                message_files_enabled = @messageFilesEnabled,
                message_images_enabled = @messageImagesEnabled,
                voice_message_enabled = @voiceMessageEnabled,
                video_message_enabled = @videoMessageEnabled
            WHERE chat_id = @chatId;";

        public const string UpdateUserSettings = @"
            UPDATE user_chat_settings ucs
            SET role = @role,
                can_add_users_by_search = @canAddUsersBySearch,
                can_add_user_by_link = @canAddUserByLink,
                can_remove_users = @canRemoveUsers,
                can_change_user_settings = @canChangeUserSettings,
                can_change_chat_settings = @canChangeChatSettings,
                can_start_calls = @canStartCalls,
                messages_enabled = @messagesEnabled,
                message_files_enabled = @messageFilesEnabled,
                message_images_enabled = @messageImagesEnabled,
                voice_message_enabled = @voiceMessageEnabled,
                video_message_enabled = @videoMessageEnabled,
                edit_messages_enabled = @editMessagesEnabled,
                delete_own_messages_enabled = @deleteOwnMessagesEnabled,
                delete_other_messages_enabled = @deleteOtherMessagesEnabled
            FROM users_chats uc
            WHERE ucs.user_chat_id = uc.id
              AND uc.chat_id = @chatId
              AND uc.user_id = @userId
              AND uc.deleted_status = FALSE;";

        public const string GetChatsByUserId = @"
            SELECT 
                c.id,
                c.creation_time,
                c.end_time,
                c.type,

                cs.allow_add_by_link,
                cs.allow_search_join,
                cs.call_enabled,
                cs.video_enabled,
                cs.message_files_enabled,
                cs.message_images_enabled,
                cs.voice_message_enabled,
                cs.video_message_enabled,

                uc.id AS user_chat_id,
                uc.user_id,
                uc.name,
                uc.join_time,

                ucs.role,
                ucs.can_add_users_by_search,
                ucs.can_add_user_by_link,
                ucs.can_remove_users,
                ucs.can_change_user_settings,
                ucs.can_change_chat_settings,
                ucs.can_start_calls,
                ucs.messages_enabled,
                ucs.message_files_enabled,
                ucs.message_images_enabled,
                ucs.voice_message_enabled,
                ucs.video_message_enabled,
                ucs.edit_messages_enabled,
                ucs.delete_own_messages_enabled,
                ucs.delete_other_messages_enabled

            FROM users_chats uc_filter

            INNER JOIN chats c 
                ON c.id = uc_filter.chat_id
                AND c.deleted_status = FALSE

            LEFT JOIN chat_settings cs
                ON cs.chat_id = c.id

            LEFT JOIN users_chats uc 
                ON uc.chat_id = c.id
                AND uc.deleted_status = FALSE

            LEFT JOIN user_chat_settings ucs
                ON ucs.user_chat_id = uc.id

            WHERE uc_filter.user_id = @userId
              AND uc_filter.deleted_status = FALSE

            ORDER BY c.creation_time DESC;";

        public const string GetChatChanges = @"
            WITH scope AS (
                SELECT 
                    uc.chat_id AS id,
                    uc.join_time,
                    uc.deleted_status,
                    uc.last_update,
                    c.end_time
                FROM users_chats uc
                JOIN chats c 
                    ON c.id = uc.chat_id
                WHERE uc.user_id = @userId
            )
            
            SELECT
                CASE
                    WHEN s.deleted_status = TRUE
                         AND s.last_update > @since
                    THEN 'deleted'
            
                    WHEN s.deleted_status = FALSE
                         AND s.join_time > @since
                    THEN 'created'
            
                    WHEN s.deleted_status = FALSE
                         AND s.join_time <= @since
                         AND (
                             s.last_update > @since
                             OR s.end_time > @since
                         )
                    THEN 'updated'
            
                    ELSE NULL
                END AS type,
            
                s.id
            
            FROM scope s
            WHERE
                (
                    s.deleted_status = TRUE
                    AND s.last_update > @since
                )
                OR
                (
                    s.deleted_status = FALSE
                    AND (
                        s.join_time > @since
                        OR s.last_update > @since
                        OR s.end_time > @since
                    )
                );
            ";

        public const string GetUserIdByChatUserId = @"
            SELECT 
                uc.user_id
            FROM users_chats uc
            WHERE uc.id = @chatUserId
              AND uc.deleted_status = FALSE";
    }
}