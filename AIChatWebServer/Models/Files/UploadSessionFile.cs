namespace AIChatWebServer.Models.Files
{
    public sealed class UploadSessionFile(
        Guid id,
        string expectedFileName,
        FileType expectedFileType,
        long expectedFileSize,
        Guid? fileId,
        UploadSessionFileStatus status,
        string? error,
        DateTime createdAt,
        DateTime updatedAt)
    {
        public Guid Id { get; } = id;
        public string ExpectedFileName { get; } = expectedFileName;
        public FileType ExpectedFileType { get; } = expectedFileType;
        public long ExpectedFileSize { get; } = expectedFileSize;
        public Guid? FileId { get; } = fileId;
        public UploadSessionFileStatus Status { get; private set; } = status;
        public string? Error { get; } = error;
        public DateTime CreatedAt { get; } = createdAt;
        public DateTime UpdatedAt { get; } = updatedAt;

        public static UploadSessionFile Create(
            Guid id,
            string expectedFileName,
            FileType expectedFileType,
            long expectedFileSize,
            DateTime now)
        {
            return new UploadSessionFile(
               id, expectedFileName,
                expectedFileType, expectedFileSize, 
                null, UploadSessionFileStatus.Pending, 
                null, now, now);
        }

        public void Upload()
        {
            Status = UploadSessionFileStatus.Uploaded;
        }

        public bool IsUploaded()
        {
            return Status == UploadSessionFileStatus.Uploaded;
        }
    }
}