using AIChatWebServer.Models.Files;

namespace AIChatWebServer.Services.Interfaces
{
    public interface IFileService
    {
        Task<Guid> UploadAsync(IFormFile file, FileType fileType, Guid userId, CancellationToken ct);
        Task<FileResult> GetById(Guid id, CancellationToken ct);
        Task DeleteAsync(Guid id, CancellationToken ct);
    }
}
