using AIChatWebServer.Models.Files;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IUploadSessionRepository
    {
        Task<Guid> CreateSessionWithFilesAsync(
            Guid userId,
            UploadSessionPurpose purpose,
            DateTime expiresAt,
            IReadOnlyCollection<UploadSessionFile> files,
            CancellationToken ct = default);

        Task SetFileUploadedAsync(
            Guid uploadSessionFileId,
            Guid fileId,
            CancellationToken ct = default);

        Task SetFileFailedAsync(
            Guid uploadSessionFileId,
            string error,
            CancellationToken ct = default);

        Task CancelFileAsync(
            Guid uploadSessionFileId,
            CancellationToken ct = default);

        Task CompleteSessionAsync(
            Guid sessionId,
            CancellationToken ct = default);

        Task CancelSessionAsync(
            Guid sessionId,
            string reason,
            CancellationToken ct = default);

        Task<UploadSession?> GetByIdAsync(
            Guid id,
            CancellationToken ct = default);

        Task BindToEntityAsync(
            Guid sessionId,
            Guid entityId,
            CancellationToken ct = default);
    }
}