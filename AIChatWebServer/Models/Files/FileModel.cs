namespace AIChatWebServer.Models.Files
{
    public sealed class FileModel(
        Guid id,
        FileType fileType,
        string fileName,
        string filePath,
        string contentType,
        long fileSize,
        string? checksum,
        Guid uploadedBy,
        DateTime createdAt,
        int referenceCount)
    {
        public Guid Id { get; init; } = id;
        public FileType FileType { get; init; } = fileType;
        public string FileName { get; init; } = fileName;
        public string FilePath { get; init; } = filePath;
        public string ContentType { get; init; } = contentType;
        public long FileSize { get; init; } = fileSize;
        public string? Checksum { get; init; } = checksum;
        public Guid UploadedBy { get; init; } = uploadedBy;
        public DateTime CreatedAt { get; init; } = createdAt;
        public int ReferenceCount { get; private set; } = referenceCount;

        public void RemoveUsage()
        {
            ReferenceCount--;
        }
    }
}