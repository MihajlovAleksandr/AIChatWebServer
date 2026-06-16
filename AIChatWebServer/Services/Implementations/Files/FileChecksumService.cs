using AIChatWebServer.Services.Interfaces.Files;
using AIChatWebServer.Services.Interfaces.Utils;

namespace AIChatWebServer.Services.Implementations.Files
{
    public sealed class FileChecksumService(
        IHasher hasher) : IFileChecksumService
    {
        private readonly IHasher _hasher = hasher;

        public async Task<string> ComputeAsync(
            string filePath,
            CancellationToken ct = default)
        {
            await using var stream =
                new FileStream(
                    filePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: 81920,
                    useAsync: true);

            return await _hasher.HashAsync(stream, ct);
        }

        public async Task<string> ComputeAsync(
            Stream stream,
            CancellationToken ct = default)
        {
            return await _hasher.HashAsync(stream, ct);
        }

        public async Task<bool> VerifyAsync(
            string filePath,
            string expectedChecksum,
            CancellationToken ct = default)
        {
            string actual =
                await ComputeAsync(filePath, ct);

            return string.Equals(
                actual,
                expectedChecksum,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}