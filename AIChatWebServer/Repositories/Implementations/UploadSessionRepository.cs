using AIChatWebServer.Models.Files;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;
using System.Data;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class UploadSessionRepository : BaseRepository, IUploadSessionRepository
    {
        public async Task<Guid> CreateSessionWithFilesAsync(
            Guid userId,
            UploadSessionPurpose purpose,
            DateTime expiresAt,
            IReadOnlyCollection<UploadSessionFile> files,
            CancellationToken ct = default)
        {
            await using var conn = await GetConnectionAsync(ct);
            await using var tx = await conn.BeginTransactionAsync(ct);

            Guid sessionId = Guid.NewGuid();

            try
            {
                await using (var cmd = new NpgsqlCommand(
                                 UploadSessionQueries.CreateSession,
                                 conn,
                                 tx))
                {
                    cmd.Parameters.AddWithValue("@id", sessionId);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@purpose", purpose.ToString());
                    cmd.Parameters.AddWithValue("@status", UploadSessionStatus.Created.ToString());
                    cmd.Parameters.AddWithValue("@expiresAt", expiresAt);

                    await cmd.ExecuteNonQueryAsync(ct);
                }

                foreach (var file in files)
                {
                    await using var cmd = new NpgsqlCommand(
                        UploadSessionQueries.AddFile,
                        conn,
                        tx);

                    cmd.Parameters.AddWithValue("@id", file.Id);
                    cmd.Parameters.AddWithValue("@sessionId", sessionId);
                    cmd.Parameters.AddWithValue("@fileName", file.ExpectedFileName);
                    cmd.Parameters.AddWithValue("@fileType", (int)file.ExpectedFileType);
                    cmd.Parameters.AddWithValue("@fileSize", file.ExpectedFileSize);
                    cmd.Parameters.AddWithValue("@status", file.Status.ToString());
                    cmd.Parameters.AddWithValue("@createdAt", file.CreatedAt);
                    cmd.Parameters.AddWithValue("@updatedAt", file.UpdatedAt);

                    await cmd.ExecuteNonQueryAsync(ct);
                }

                await tx.CommitAsync(ct);

                return sessionId;
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }

        public Task SetFileUploadedAsync(
            Guid uploadSessionFileId,
            Guid fileId,
            CancellationToken ct = default) =>
            ExecuteAsync(
                UploadSessionQueries.SetFileUploaded,
                ct,
                ("@id", uploadSessionFileId),
                ("@fileId", fileId));

        public Task SetFileFailedAsync(
            Guid uploadSessionFileId,
            string error,
            CancellationToken ct = default) =>
            ExecuteAsync(
                UploadSessionQueries.SetFileFailed,
                ct,
                ("@id", uploadSessionFileId),
                ("@error", error));

        public Task CancelFileAsync(
            Guid uploadSessionFileId,
            CancellationToken ct = default) =>
            ExecuteAsync(
                UploadSessionQueries.CancelFile,
                ct,
                ("@id", uploadSessionFileId));

        public Task CompleteSessionAsync(
            Guid sessionId,
            CancellationToken ct = default) =>
            ExecuteAsync(
                UploadSessionQueries.CompleteSession,
                ct,
                ("@id", sessionId));

        public Task CancelSessionAsync(
            Guid sessionId,
            string reason,
            CancellationToken ct = default) =>
            ExecuteAsync(
                UploadSessionQueries.CancelSession,
                ct,
                ("@id", sessionId),
                ("@reason", reason));

        public async Task<UploadSession?> GetByIdAsync(
            Guid id,
            CancellationToken ct = default)
        {
            await using var conn = await GetConnectionAsync(ct);
            await using var cmd = new NpgsqlCommand(
                UploadSessionQueries.GetByIdWithFiles, conn);

            cmd.Parameters.AddWithValue("@id", id);

            await using var reader = await cmd.ExecuteReaderAsync(ct);

            UploadSession? session = null;
            List<UploadSessionFile>? files = null;

            while (await reader.ReadAsync(ct))
            {
                if (session == null)
                {
                    files = new List<UploadSessionFile>();

                    session = new UploadSession(
                        reader.GetGuid(reader.GetOrdinal("id")),
                        reader.GetGuid(reader.GetOrdinal("user_id")),
                        Enum.Parse<UploadSessionPurpose>(reader.GetString(reader.GetOrdinal("purpose"))),
                        reader.IsDBNull(reader.GetOrdinal("entity_id"))
                            ? null
                            : reader.GetGuid(reader.GetOrdinal("entity_id")),
                        Enum.Parse<UploadSessionStatus>(reader.GetString(reader.GetOrdinal("status"))),
                        reader.IsDBNull(reader.GetOrdinal("cancel_reason"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("cancel_reason")),
                        reader.GetDateTime(reader.GetOrdinal("expires_at")),
                        reader.IsDBNull(reader.GetOrdinal("completed_at"))
                            ? null
                            : reader.GetDateTime(reader.GetOrdinal("completed_at")),
                        reader.GetDateTime(reader.GetOrdinal("created_at")),
                        reader.GetDateTime(reader.GetOrdinal("updated_at")),
                        files
                    );
                }

                if (!reader.IsDBNull(reader.GetOrdinal("usf_id")))
                {
                    files!.Add(MapFile(reader));
                }
            }

            return session;
        }

        public Task BindToEntityAsync(
            Guid sessionId,
            Guid entityId,
            CancellationToken ct = default) =>
            ExecuteAsync(
                UploadSessionQueries.BindToEntity,
                ct,
                ("@id", sessionId),
                ("@entityId", entityId));

        private static UploadSessionFile MapFile(IDataRecord r) =>
            new UploadSessionFile(
                r.GetGuid(r.GetOrdinal("usf_id")),
                r.GetString(r.GetOrdinal("expected_file_name")),
                (FileType)r.GetInt32(r.GetOrdinal("expected_file_type")),
                r.GetInt64(r.GetOrdinal("expected_file_size")),
                r.IsDBNull(r.GetOrdinal("file_id"))
                    ? null
                    : r.GetGuid(r.GetOrdinal("file_id")),
                Enum.Parse<UploadSessionFileStatus>(r.GetString(r.GetOrdinal("usf_status"))),
                r.IsDBNull(r.GetOrdinal("error"))
                    ? null
                    : r.GetString(r.GetOrdinal("error")),
                r.GetDateTime(r.GetOrdinal("usf_created_at")),
                r.GetDateTime(r.GetOrdinal("usf_updated_at"))
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
            await using var conn = await GetConnectionAsync(ct);
            await using var cmd = new NpgsqlCommand(sql, conn);

            AddParameters(cmd, p);

            await cmd.ExecuteNonQueryAsync(ct);
        }
    }
}