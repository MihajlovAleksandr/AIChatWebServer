namespace AIChatWebServer.Repositories.Constants
{
    public static class FileQueries
    {
        public const string CreateFile = @"
            INSERT INTO files (
                id,
                file_type,
                file_name,
                file_path,
                content_type,
                file_size,
                checksum,
                uploaded_by,
                created_at,
                deleted_status,
                reference_count
            )
            VALUES (
                @id,
                @fileType,
                @fileName,
                @filePath,
                @contentType,
                @fileSize,
                @checksum,
                @uploadedBy,
                NOW(),
                false,
                0
            );
        ";

        public const string GetById = @"
            SELECT
                id,
                file_type,
                file_name,
                file_path,
                content_type,
                file_size,
                checksum,
                uploaded_by,
                created_at,
                deleted_status,
                reference_count
            FROM files
            WHERE id = @id
              AND deleted_status = false;
        ";

        public const string GetByPath = @"
            SELECT
                id,
                file_type,
                file_name,
                file_path,
                content_type,
                file_size,
                checksum,
                uploaded_by,
                created_at,
                deleted_status,
                reference_count
            FROM files
            WHERE file_path = @filePath
              AND deleted_status = false;
        ";

        public const string GetByUser = @"
            SELECT
                id,
                file_type,
                file_name,
                file_path,
                content_type,
                file_size,
                checksum,
                uploaded_by,
                created_at,
                deleted_status,
                reference_count
            FROM files
            WHERE uploaded_by = @uploadedBy
              AND deleted_status = false;
        ";

        public const string TrySoftDelete = @"
            UPDATE files
            SET deleted_status = true
            WHERE id = @id
              AND reference_count = 0
              AND deleted_status = false;
        ";

        public const string Delete = @"
            DELETE FROM files
            WHERE id = @id
              AND deleted_status = true;
        ";
    }
}