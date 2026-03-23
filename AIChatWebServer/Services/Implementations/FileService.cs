using AIChatWebServer.Models.Files;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces;
using AIChatWebServer.Models.Exceptions.Implementations.File;
using FileNotFoundException = AIChatWebServer.Models.Exceptions.Implementations.File.FileNotFoundException;

namespace AIChatWebServer.Services.Implementations
{
    public class FileService(
        IFileStorage fileStorage,
        IFileRepository fileRepository) : IFileService
    {
        private readonly IFileStorage _fileStorage = fileStorage;
        private readonly IFileRepository _fileRepository = fileRepository;

        public async Task<Guid> UploadAsync(
            IFormFile file,
            FileType fileType,
            Guid userId,
            CancellationToken ct)
        {
            Guid id = Guid.NewGuid();
            string filePath = _fileStorage.GetPath(fileType, id);

            string checksum =
                await _fileStorage.UploadAsync(file, filePath, ct);

            await _fileRepository.CreateAsync(
                id,
                fileType,
                file.FileName,
                filePath,
                file.ContentType,
                file.Length,
                checksum,
                userId,
                ct);

            return id;
        }

        public async Task<FileResult> GetById(
            Guid id,
            CancellationToken ct)
        {
            var fileInfo =
                await _fileRepository.GetByIdAsync(id, ct)
                ?? throw new FileNotFoundException(id);

            return new FileResult(await _fileStorage.GetAsync(fileInfo.FilePath, ct),
                fileInfo.ContentType, fileInfo.FileName, fileInfo.FileType);
        }

        public async Task DeleteAsync(
            Guid id,
            CancellationToken ct)
        {
            bool deleted =
                await _fileRepository.TrySoftDeleteAsync(id, ct);

            if (!deleted)
                throw new FileDeleteConflictException(id);

            var file =
                await _fileRepository.GetByIdAsync(id, ct)
                ?? throw new FileNotFoundException(id);

            await _fileStorage.DeleteAsync(file.FilePath, ct);
        }
    }
}