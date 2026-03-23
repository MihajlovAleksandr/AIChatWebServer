using AIChatWebServer.Models.Files;
using AIChatWebServer.Services.Interfaces;

namespace AIChatWebServer.Services.Implementations
{
    public class FileStorage(
        IConfiguration configuration,
        IFileChecksumService fileChecksumService,
        ILogger<FileStorage> logger) : IFileStorage
    {
        private readonly string _directoryPath = configuration["Files:DirectoryPath"]
            ?? throw new ArgumentException("File directory is not configured");

        private readonly IFileChecksumService _fileChecksumService = fileChecksumService
            ?? throw new ArgumentNullException(nameof(fileChecksumService));

        private readonly ILogger<FileStorage> _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));

        public async Task<string> UploadAsync(
            IFormFile file,
            string path,
            CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(file);
            ArgumentNullException.ThrowIfNull(path);

            string fullPath = Path.Combine(_directoryPath, path);

            _logger.LogInformation("Uploading file to {FullPath}", fullPath);

            try
            {
                Directory.CreateDirectory(
                    Path.GetDirectoryName(fullPath)!);

                await using var stream = new FileStream(
                    fullPath,
                    FileMode.CreateNew,
                    FileAccess.ReadWrite,
                    FileShare.None,
                    bufferSize: 81920,
                    useAsync: true);

                await file.CopyToAsync(stream, ct);

                stream.Position = 0;

                string checksum = await _fileChecksumService.ComputeAsync(stream, ct);

                _logger.LogInformation(
                    "File uploaded successfully to {FullPath} with checksum {Checksum}",
                    fullPath,
                    checksum);

                return checksum;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file to {FullPath}", fullPath);
                throw;
            }
        }

        public Task<Stream> GetAsync(
            string path,
            CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(path);

            string fullPath = Path.Combine(_directoryPath, path);

            _logger.LogInformation("Getting file from {FullPath}", fullPath);

            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("File not found: {FullPath}", fullPath);
                throw new FileNotFoundException($"File not found: {path}");
            }

            Stream stream = new FileStream(
                fullPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 81920,
                useAsync: true);

            return Task.FromResult(stream);
        }

        public Task<bool> ExistsAsync(
            string path,
            CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(path);

            string fullPath = Path.Combine(_directoryPath, path);

            bool exists = File.Exists(fullPath);

            _logger.LogDebug("Checking file exists at {FullPath}: {Exists}", fullPath, exists);

            return Task.FromResult(exists);
        }

        public async Task<bool> VerifyAsync(
            string path,
            string expectedChecksum,
            CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(path);
            ArgumentNullException.ThrowIfNull(expectedChecksum);

            string fullPath = Path.Combine(_directoryPath, path);

            _logger.LogInformation("Verifying file {FullPath}", fullPath);

            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("File not found during verification: {FullPath}", fullPath);
                return false;
            }

            bool result = await _fileChecksumService.VerifyAsync(
                fullPath,
                expectedChecksum,
                ct);

            _logger.LogInformation("Verification result for {FullPath}: {Result}", fullPath, result);

            return result;
        }

        public string GetPath(FileType fileType, Guid fileId)
        {
            string path = Path.Combine(fileType.ToString(), fileId.ToString());

            _logger.LogDebug("Generated path {Path} for fileType {FileType} and fileId {FileId}",
                path, fileType, fileId);

            return path;
        }

        public Task DeleteAsync(
            string path,
            CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(path);

            string fullPath = Path.Combine(_directoryPath, path);

            _logger.LogInformation("Deleting file {FullPath}", fullPath);

            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("File not found for deletion: {FullPath}", fullPath);
                return Task.CompletedTask;
            }

            try
            {
                File.Delete(fullPath);

                _logger.LogInformation("File deleted: {FullPath}", fullPath);

                CleanupEmptyDirectories(
                    Path.GetDirectoryName(fullPath));
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "IO error while deleting file {FullPath}", fullPath);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Access denied while deleting file {FullPath}", fullPath);
            }

            return Task.CompletedTask;
        }

        private void CleanupEmptyDirectories(string? directoryPath)
        {
            while (!string.IsNullOrEmpty(directoryPath)
                   && directoryPath.StartsWith(_directoryPath))
            {
                if (!Directory.Exists(directoryPath))
                    return;

                if (Directory.EnumerateFileSystemEntries(directoryPath).Any())
                    return;

                try
                {
                    Directory.Delete(directoryPath);

                    _logger.LogDebug("Deleted empty directory {DirectoryPath}", directoryPath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deleting directory {DirectoryPath}", directoryPath);
                    return;
                }

                directoryPath = Path.GetDirectoryName(directoryPath);
            }
        }
    }
}