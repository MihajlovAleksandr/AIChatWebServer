using AIChatWebServer.Models.Files;

namespace AIChatWebServer.Services.Interfaces.Files
{
    public interface IUploadSessionService
    {
        Task<UploadSession> CreateSessionAsync(
            Guid userId, UploadSessionPurpose purpose,
            IReadOnlyCollection<UploadSessionFile> files,
            CancellationToken ct);

        Task<UploadSession> GetByIdAsync(
            Guid sessionId, 
            CancellationToken ct);

        Task SetFileUploadedAsync(
            Guid uploadSessionFileId,
            Guid fileId, Guid sessionId, 
            CancellationToken ct = default);

        Task SetFileFailedAsync(
            Guid uploadSessionFileId,
            Guid sessionId, string error,
            CancellationToken ct = default);

        Task CancelFileAsync(
            Guid uploadSessionFileId,
            Guid sessionId, 
            CancellationToken ct = default);

        Task CompleteSessionAsync(
            Guid sessionId,
            CancellationToken ct = default);

        Task CancelSessionAsync(
            Guid sessionId,
            string reason,
            CancellationToken ct = default);

        Task BindToEntityAsync(
            Guid sessionId, Guid entityId,
            UploadSessionPurpose purpose, 
            CancellationToken ct);
    }
}
