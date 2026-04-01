using AIChatWebServer.Models.Exceptions.Implementations.File;
using AIChatWebServer.Models.Files;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces;

namespace AIChatWebServer.Services.Implementations
{
    public class UploadSessionService(IUploadSessionRepository uploadSessionRepository, IUploadSessionTtlCalculator uploadSessionTtlCalculator) : IUploadSessionService
    {                                 
        private readonly IUploadSessionRepository _uploadSessionRepository = uploadSessionRepository;
        private readonly IUploadSessionTtlCalculator _uploadSessionTtlCalculator = uploadSessionTtlCalculator;

        public async Task<UploadSession> CreateSessionAsync(
            Guid userId, UploadSessionPurpose purpose,
            IReadOnlyCollection<UploadSessionFile> files,
            CancellationToken ct)
        {
            Guid sessionId =
                await _uploadSessionRepository.CreateSessionWithFilesAsync(
                    userId, purpose,
                    _uploadSessionTtlCalculator.CalculateExpiration(files, DateTime.UtcNow),
                    files, ct);

            return await _uploadSessionRepository.GetByIdAsync(sessionId, ct)
                ?? throw new UploadSessionNotFoundException(sessionId);
        }

        public async Task<UploadSession> GetByIdAsync(Guid sessionId, CancellationToken ct)
        {
            return await _uploadSessionRepository.GetByIdAsync(sessionId, ct) 
                ?? throw new UploadSessionNotFoundException(sessionId);
        }

        public async Task BindToEntityAsync(Guid sessionId, Guid entityId, 
            UploadSessionPurpose purpose, CancellationToken ct)
        {
            UploadSession uploadSession = await GetByIdAsync(sessionId, ct);
            if (uploadSession.EntityId != null)
                throw new UploadSessionEntityAlreadyAssignedException(sessionId, uploadSession.EntityId.Value);
            if(uploadSession.Purpose!=purpose)
                throw new UploadSessionInvalidPurposeException(sessionId, uploadSession.Purpose);
            await _uploadSessionRepository.BindToEntityAsync(sessionId, entityId, ct);
        }

        public async Task SetFileUploadedAsync(Guid uploadSessionFileId, Guid fileId, Guid sessionId, CancellationToken ct = default)
        {
            UploadSession session = await GetByIdAsync(sessionId, ct);
            if (session.Status == UploadSessionStatus.Completed)
                throw new UploadSessionAlreadyCompletedException(sessionId);
            if (session.Status == UploadSessionStatus.Expired)
                throw new UploadSessionExpiredException(sessionId);
            if(session.Status == UploadSessionStatus.Canceled)
                throw new UploadSessionCanceledException(sessionId);

            UploadSessionFile sessionFile = session.GetFileById(uploadSessionFileId);
            if (sessionFile.Status == UploadSessionFileStatus.Uploaded)
                throw new UploadSessionFileAlreadyUploadedException(sessionFile.Id);
            if (sessionFile.Status == UploadSessionFileStatus.Canceled)
                throw new UploadSessionFileCanceledException(sessionFile.Id);
            if(sessionFile.Status == UploadSessionFileStatus.Failed)
                throw new UploadSessionFileFailedException(sessionFile.Id);

            await _uploadSessionRepository.SetFileUploadedAsync(uploadSessionFileId, fileId, ct);
        }

        public async Task SetFileFailedAsync(Guid uploadSessionFileId, Guid sessionId, string error, CancellationToken ct = default)
        {
            UploadSession session = await GetByIdAsync(sessionId, ct);
            if (session.Status == UploadSessionStatus.Completed)
                throw new UploadSessionAlreadyCompletedException(sessionId);
            if (session.Status == UploadSessionStatus.Expired)
                throw new UploadSessionExpiredException(sessionId);
            if (session.Status == UploadSessionStatus.Canceled)
                throw new UploadSessionCanceledException(sessionId);

            UploadSessionFile sessionFile = session.GetFileById(uploadSessionFileId);
            if (sessionFile.Status == UploadSessionFileStatus.Uploaded)
                throw new UploadSessionFileAlreadyUploadedException(sessionFile.Id);
            if (sessionFile.Status == UploadSessionFileStatus.Canceled)
                throw new UploadSessionFileCanceledException(sessionFile.Id);
            if (sessionFile.Status == UploadSessionFileStatus.Uploaded)
                throw new UploadSessionFileAlreadyUploadedException(sessionFile.Id);

            await _uploadSessionRepository.SetFileFailedAsync(uploadSessionFileId, error, ct);
        }

        public async Task CancelFileAsync(Guid uploadSessionFileId, Guid sessionId, CancellationToken ct = default)
        {
            UploadSession session = await GetByIdAsync(sessionId, ct);
            if (session.Status == UploadSessionStatus.Completed)
                throw new UploadSessionAlreadyCompletedException(sessionId);
            if (session.Status == UploadSessionStatus.Expired)
                throw new UploadSessionExpiredException(sessionId);
            if (session.Status == UploadSessionStatus.Canceled)
                throw new UploadSessionCanceledException(sessionId);

            UploadSessionFile sessionFile = session.GetFileById(uploadSessionFileId);
            if (sessionFile.Status == UploadSessionFileStatus.Uploaded)
                throw new UploadSessionFileAlreadyUploadedException(sessionFile.Id);
            if (sessionFile.Status == UploadSessionFileStatus.Failed)
                throw new UploadSessionFileFailedException(sessionFile.Id);
            if (sessionFile.Status == UploadSessionFileStatus.Uploaded)
                throw new UploadSessionFileAlreadyUploadedException(sessionFile.Id);

            await _uploadSessionRepository.CancelFileAsync(uploadSessionFileId, ct);
        }

        public async Task CompleteSessionAsync(Guid sessionId, CancellationToken ct = default)
        {
            UploadSession session = await GetByIdAsync(sessionId, ct);
            if (session.Status == UploadSessionStatus.Completed)
                throw new UploadSessionAlreadyCompletedException(sessionId);
            if (session.Status == UploadSessionStatus.Expired)
                throw new UploadSessionExpiredException(sessionId);
            if (session.Status == UploadSessionStatus.Canceled)
                throw new UploadSessionCanceledException(sessionId);

            if (session.IsUploaded())
            {
                await _uploadSessionRepository.CompleteSessionAsync(sessionId, ct);
            }
            else
                throw new UploadSessionFilesNotFullyUploadedException(sessionId);
        }

        public async Task CancelSessionAsync(Guid sessionId, string reason, CancellationToken ct = default)
        {
            UploadSession session = await GetByIdAsync(sessionId, ct);
            if (session.Status == UploadSessionStatus.Completed)
                throw new UploadSessionAlreadyCompletedException(sessionId);
            if (session.Status == UploadSessionStatus.Expired)
                throw new UploadSessionExpiredException(sessionId);
            if (session.Status == UploadSessionStatus.Canceled)
                throw new UploadSessionCanceledException(sessionId);

            await _uploadSessionRepository.CancelSessionAsync(sessionId, reason, ct);
        }
    }
}
