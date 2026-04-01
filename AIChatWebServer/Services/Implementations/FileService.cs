using AIChatWebServer.Models.Exceptions.Implementations.File;
using AIChatWebServer.Models.Files;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces;
using FileNotFoundException = AIChatWebServer.Models.Exceptions.Implementations.File.FileNotFoundException;

namespace AIChatWebServer.Services.Implementations
{
    public class FileService(
        IFileStorage fileStorage,
        IFileRepository fileRepository, 
        IUploadSessionService uploadSessionService) : IFileService
    {
        private readonly IFileStorage _fileStorage = fileStorage;
        private readonly IFileRepository _fileRepository = fileRepository;
        private readonly IUploadSessionService _uploadSessionService = uploadSessionService;

        public async Task<Guid> UploadAsync(
            Guid uploadSessionId,
            Guid uploadSessionFileId,
            IFormFile file,
            FileType fileType,
            Guid userId,
            CancellationToken ct)
        {
            UploadSession session = await _uploadSessionService.GetByIdAsync(uploadSessionId, ct);
            UploadSessionFile uploadFile = session.GetFileById(uploadSessionFileId);

            if (uploadFile.ExpectedFileType != fileType)
                throw new UploadSessionFileInvalidTypeException(
                    uploadSessionFileId,
                    uploadFile.ExpectedFileType,
                    fileType);

            if (uploadFile.ExpectedFileSize != file.Length)
                throw new UploadSessionFileInvalidSizeException(
                    uploadSessionFileId,
                    uploadFile.ExpectedFileSize,
                    file.Length);

            if (uploadFile.ExpectedFileName != file.FileName)
                throw new UploadSessionFileInvalidNameException(
                    uploadSessionFileId,
                    uploadFile.ExpectedFileName,
                    file.FileName);

            Guid id = await UploadFileAsync(file, fileType, userId, ct);
            await _uploadSessionService.SetFileUploadedAsync(uploadSessionFileId, id, uploadSessionId, ct);
            uploadFile.Upload();
            if (session.IsUploaded())
            {
                await _uploadSessionService.CompleteSessionAsync(session.Id, ct);
            }

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

            var file =
                await _fileRepository.GetByIdAsync(id, ct)
                ?? throw new FileNotFoundException(id);

            bool deleted =
                await _fileRepository.TrySoftDeleteAsync(id, ct);

            if (!deleted)
                throw new FileDeleteConflictException(id);

            await _fileStorage.DeleteAsync(file.FilePath, ct);
        }

        private async Task<Guid> UploadFileAsync(
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
    }
}