namespace AIChatWebServer.Repositories.Constants
{
    public static class UploadSessionQueries
    {
        public const string CreateSession = @"
            INSERT INTO upload_sessions (
                id,
                user_id,
                purpose,
                entity_id,
                status,
                expires_at,
                created_at,
                updated_at
            )
            VALUES (
                @id,
                @userId,
                @purpose,
                NULL,
                @status,
                @expiresAt,
                NOW(),
                NOW()
            );";

        public const string AddFile = @"
            INSERT INTO upload_session_files (
                id,
                session_id,
                expected_file_name,
                expected_file_type,
                expected_file_size,
                status,
                created_at,
                updated_at
            )
            VALUES (
                @id,
                @sessionId,
                @fileName,
                @FileType,
                @fileSize,
                @status,
                @createdAt,
                @updatedAt
            );";

        public const string SetFileUploaded = @"
            UPDATE upload_session_files
            SET file_id = @fileId,
                status = 'Uploaded',
                updated_at = NOW()
            WHERE id = @id;

            UPDATE upload_sessions
            SET status = 'InProgress',
                updated_at = NOW()
            WHERE id = (
                SELECT session_id
                FROM upload_session_files
                WHERE id = @id
            );";

        public const string SetFileFailed = @"
            UPDATE upload_session_files
            SET status = 'Failed',
                error = @error,
                updated_at = NOW()
            WHERE id = @id;";

        public const string CancelFile = @"
            UPDATE upload_session_files
            SET status = 'Canceled',
                updated_at = NOW()
            WHERE id = @id;";

        public const string CompleteSession = @"
            UPDATE upload_sessions
            SET status = 'Completed',
                completed_at = NOW(),
                updated_at = NOW()
            WHERE id = @id;";

        public const string CancelSession = @"
            UPDATE upload_sessions
            SET status = 'Canceled',
                cancel_reason = @reason,
                updated_at = NOW()
            WHERE id = @id;";

        public const string GetByIdWithFiles = @"
            SELECT
                s.id,
                s.user_id,
                s.purpose,
                s.entity_id,
                s.status,
                s.cancel_reason,
                s.expires_at,
                s.completed_at,
                s.created_at,
                s.updated_at,
        
                usf.id AS usf_id,
                usf.expected_file_name,
                usf.expected_file_type,
                usf.expected_file_size,
                usf.file_id,
                usf.status AS usf_status,
                usf.error,
                usf.created_at AS usf_created_at,
                usf.updated_at AS usf_updated_at
        
            FROM upload_sessions s
            LEFT JOIN upload_session_files usf
                ON usf.session_id = s.id
        
            WHERE s.id = @id;
        ";

        public const string GetFilesBySession = @"
            SELECT *
            FROM upload_session_files
            WHERE session_id = @sessionId;";

        public const string BindToEntity = @"
            UPDATE upload_sessions
            SET entity_id = @entityId,
                updated_at = NOW()
            WHERE id = @id
            AND entity_id IS NULL;
        ";
    }
}