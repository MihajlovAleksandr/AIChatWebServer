using AIChatWebServer.Models.Files;

namespace AIChatWebServer.Services.Interfaces.Files
{
    public interface IFileStorage
    {
        Task<string> UploadAsync(IFormFile file, string path, CancellationToken ct);
        Task<Stream> GetAsync(string path, CancellationToken ct = default);
        Task<bool> ExistsAsync(string path, CancellationToken ct = default);
        Task<bool> VerifyAsync(string path, string expectedChecksum, CancellationToken ct = default);
        Task DeleteAsync(string path, CancellationToken ct = default);
        string GetPath(FileType fileType, Guid fileId);
    }
}
