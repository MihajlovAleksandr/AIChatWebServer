using AIChatWebServer.Models.Exceptions.Implementations.File;

namespace AIChatWebServer.Models.Files
{
    public sealed class UploadSession(
        Guid id,
        Guid userId,
        UploadSessionPurpose purpose,
        Guid? entityId,
        UploadSessionStatus status,
        string? cancelReason,
        DateTime expiresAt,
        DateTime? completedAt,
        DateTime createdAt,
        DateTime updatedAt,
        IReadOnlyList<UploadSessionFile> files)
    {
        public Guid Id { get; } = id;
        public Guid UserId { get; } = userId;
        public UploadSessionPurpose Purpose { get; } = purpose;
        public Guid? EntityId { get; } = entityId;

        public UploadSessionStatus Status { get; } = status;
        public string? CancelReason { get; } = cancelReason;

        public DateTime ExpiresAt { get; } = expiresAt;
        public DateTime? CompletedAt { get; } = completedAt;

        public DateTime CreatedAt { get; } = createdAt;
        public DateTime UpdatedAt { get; } = updatedAt;

        public IReadOnlyList<UploadSessionFile> Files { get; } = files;

        public UploadSessionFile GetFileById(Guid id)
        {
            return Files.FirstOrDefault(f => f.Id == id)
                ?? throw new FileDoesNotBelongToSessionException(id, Id);
        }

        public bool IsUploaded()
        {
            return Files.All(f => f.IsUploaded());
        }
    }
}