using AIChatWebServer.Models.Files;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;
using System.Data;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class FileRepository : BaseRepository, IFileRepository
    {
        public async Task<Guid> CreateAsync(
            Guid id,
            FileType fileType,
            string fileName,
            string filePath,
            string contentType,
            long fileSize,
            string? checksum,
            Guid uploadedBy,
            CancellationToken ct = default)
        {
            await using var conn =
                await GetConnectionAsync(ct);

            await using var cmd =
                new NpgsqlCommand(FileQueries.CreateFile, conn);

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@fileType", (int)fileType);
            cmd.Parameters.AddWithValue("@fileName", fileName);
            cmd.Parameters.AddWithValue("@filePath", filePath);
            cmd.Parameters.AddWithValue("@contentType", contentType);
            cmd.Parameters.AddWithValue("@fileSize", fileSize);
            cmd.Parameters.AddWithValue("@checksum",
                checksum ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@uploadedBy", uploadedBy);

            await cmd.ExecuteNonQueryAsync(ct);

            return id;
        }

        public Task<FileModel?> GetByIdAsync(
            Guid id,
            CancellationToken ct = default) =>
            GetSingleAsync(
                FileQueries.GetById,
                ct,
                ("@id", id));

        public Task<FileModel?> GetByPathAsync(
            string filePath,
            CancellationToken ct = default) =>
            GetSingleAsync(
                FileQueries.GetByPath,
                ct,
                ("@filePath", filePath));

        public Task<IReadOnlyList<FileModel>> GetByUserAsync(
            Guid userId,
            CancellationToken ct = default) =>
            GetListAsync(
                FileQueries.GetByUser,
                ct,
                ("@uploadedBy", userId));

        public Task DeleteAsync(
            Guid id,
            CancellationToken ct = default) =>
            ExecuteAsync(
                FileQueries.Delete,
                ct,
                ("@id", id));

        public Task<bool> TrySoftDeleteAsync(
            Guid id,
            CancellationToken ct = default) =>
            ExecuteWithResultAsync(
                FileQueries.TrySoftDelete,
                ct,
                ("@id", id));

        private async Task<FileModel?> GetSingleAsync(
            string sql,
            CancellationToken ct,
            params (string, object)[] p)
        {
            var list = await ReadAsync(sql, ct, p);
            return list.FirstOrDefault();
        }

        private async Task<IReadOnlyList<FileModel>> GetListAsync(
            string sql,
            CancellationToken ct,
            params (string, object)[] p)
        {
            return await ReadAsync(sql, ct, p);
        }

        private async Task<List<FileModel>> ReadAsync(
            string sql,
            CancellationToken ct,
            params (string, object)[] parameters)
        {
            var result = new List<FileModel>();

            await using var conn =
                await GetConnectionAsync(ct);

            await using var cmd =
                new NpgsqlCommand(sql, conn);

            AddParameters(cmd, parameters);

            await using var reader =
                await cmd.ExecuteReaderAsync(ct);

            while (await reader.ReadAsync(ct))
            {
                result.Add(Map(reader));
            }

            return result;
        }

        private static FileModel Map(IDataRecord r) =>
            new FileModel(
                r.GetGuid(r.GetOrdinal("id")),
                (FileType)r.GetInt32(r.GetOrdinal("file_type")),
                r.GetString(r.GetOrdinal("file_name")),
                r.GetString(r.GetOrdinal("file_path")),
                r.GetString(r.GetOrdinal("content_type")),
                r.GetInt64(r.GetOrdinal("file_size")),
                r.IsDBNull(r.GetOrdinal("checksum"))
                    ? null
                    : r.GetString(r.GetOrdinal("checksum")),
                r.GetGuid(r.GetOrdinal("uploaded_by")),
                r.GetDateTime(r.GetOrdinal("created_at")),
                r.GetInt32(r.GetOrdinal("reference_count"))
            );

        private static void AddParameters(
            NpgsqlCommand cmd,
            params (string, object)[] parameters)
        {
            foreach (var (name, value) in parameters)
            {
                cmd.Parameters.AddWithValue(
                    name,
                    value ?? DBNull.Value);
            }
        }

        private async Task ExecuteAsync(
            string sql,
            CancellationToken ct,
            params (string, object)[] p)
        {
            await using var conn =
                await GetConnectionAsync(ct);

            await using var cmd =
                new NpgsqlCommand(sql, conn);

            AddParameters(cmd, p);

            await cmd.ExecuteNonQueryAsync(ct);
        }

        private async Task<bool> ExecuteWithResultAsync(
            string sql,
            CancellationToken ct,
            params (string, object)[] p)
        {
            await using var conn =
                await GetConnectionAsync(ct);

            await using var cmd =
                new NpgsqlCommand(sql, conn);

            AddParameters(cmd, p);

            int affected =
                await cmd.ExecuteNonQueryAsync(ct);

            return affected > 0;
        }
    }
}