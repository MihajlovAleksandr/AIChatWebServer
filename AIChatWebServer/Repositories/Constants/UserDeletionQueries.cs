namespace AIChatWebServer.Repositories.Constants
{
    public static class UserDeletionQueries
    {
        public const string EnsureDeletedAccount = @"
            INSERT INTO users (id, email, registration_state, deleted_status)
            VALUES (@deletedUserId, NULL, 0, true)
            ON CONFLICT (id) DO NOTHING;
        ";

        public const string DeleteConnections = @"
            DELETE FROM connections WHERE user_id = @userId;
        ";

        public const string DeleteVerificationCodes = @"
            DELETE FROM verification_codes WHERE user_id = @userId;
        ";

        public const string DeleteAuthIdentities = @"
            DELETE FROM auth_identities WHERE user_id = @userId;
        ";

        public const string DeleteUploadSessions = @"
            DELETE FROM upload_sessions WHERE user_id = @userId;
        ";

        public const string DeleteMatchmaking = @"
            DELETE FROM matchmaking_queue WHERE user_id = @userId;
        ";

        public const string DeleteGroupChatSearch = @"
            DELETE FROM group_chat_search WHERE user_id = @userId;
        ";

        public const string DeleteUserData = @"
            DELETE FROM user_data WHERE user_id = @userId;
        ";

        public const string DeletePreferences = @"
            DELETE FROM preferences WHERE user_id = @userId;
        ";

        public const string DeleteUserLanguages = @"
            DELETE FROM user_languages WHERE user_id = @userId;
        ";

        public const string DeleteNotifications = @"
            DELETE FROM user_notification_settings WHERE user_id = @userId;
        ";

        public const string DeleteMessageStatuses = @"
            DELETE FROM message_statuses WHERE user_id = @userId;
        ";

        public const string DeleteUserChatSettings = @"
            DELETE FROM user_chat_settings
            WHERE user_chat_id IN (
                SELECT id FROM users_chats WHERE user_id = @userId
            );
        ";

        public const string SoftDeleteUsersChats = @"
            UPDATE users_chats
            SET deleted_status = true
            WHERE user_id = @userId;
        ";

        public const string AnonMessages = @"
            UPDATE messages
            SET user_id = @deletedUserId
            WHERE user_id = @userId;
        ";

        public const string AnonPayments = @"
            UPDATE payment
            SET user_id = @deletedUserId
            WHERE user_id = @userId;
        ";

        public const string AnonPremium = @"
            UPDATE users_premium
            SET user_id = @deletedUserId
            WHERE user_id = @userId;
        ";

        public const string AnonReports = @"
            UPDATE reports
            SET user_id = @deletedUserId
            WHERE user_id = @userId;
        ";

        public const string AnonBans = @"
            UPDATE users_bans
            SET user_id = @deletedUserId
            WHERE user_id = @userId;
        ";

        public const string AnonFiles = @"
            UPDATE files
            SET uploaded_by = @deletedUserId
            WHERE uploaded_by = @userId;
        ";

        public const string AnonLinks = @"
            UPDATE links
            SET created_by = @deletedUserId
            WHERE created_by = @userId;
        ";

        public const string UpdateAdmins = @"
            UPDATE admins
            SET user_id = NULL,
                delete_status = true
            WHERE user_id = @userId;
        ";

        public const string FinalizeUser = @"
            UPDATE users
            SET 
                email = NULL,
                region_code = NULL,
                registration_state = 0,
                deleted_status = true
            WHERE id = @userId;
        ";

        public const string GetUserMeta = @"
            SELECT id, deleted_status
            FROM users
            WHERE id = @userId
            LIMIT 1;
        ";
    }
}