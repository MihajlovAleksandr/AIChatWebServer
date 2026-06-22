using AIChatWebServer.Models.Files;

namespace AIChatWebServer.Services.Interfaces.Files
{
    public interface IFileService
    {
        Task<Guid> UploadAsync(
            Guid uploadSessionId,
            Guid uploadSessionFileId,
            IFormFile file,
            FileType fileType,
            Guid userId,
            CancellationToken ct);
        Task<FileResult> GetById(Guid id, CancellationToken ct);
        Task DeleteAsync(Guid id, CancellationToken ct);
    }
}
