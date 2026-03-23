using AIChatWebServer.Models.Files;
using AIChatWebServer.Repositories.Constants;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IFileRepository
    {
        Task<Guid> CreateAsync(
                    Guid id,
                    FileType fileType,
                    string fileName,
                    string filePath,
                    string contentType,
                    long fileSize,
                    string? checksum,
                    Guid uploadedBy,
                    CancellationToken ct = default);

        Task<FileModel?> GetByIdAsync(
            Guid id,
            CancellationToken ct = default);

        Task<FileModel?> GetByPathAsync(
            string filePath,
            CancellationToken ct = default);

        Task<IReadOnlyList<FileModel>> GetByUserAsync(
            Guid userId,
            CancellationToken ct = default);

        Task<bool> TrySoftDeleteAsync(
            Guid id,
            CancellationToken ct = default);

        Task DeleteAsync(
            Guid id,
            CancellationToken ct = default);
    }
}
