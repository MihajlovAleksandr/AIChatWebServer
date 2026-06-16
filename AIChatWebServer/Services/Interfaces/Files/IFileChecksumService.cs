namespace AIChatWebServer.Services.Interfaces.Files
{
    public interface IFileChecksumService
    {
        Task<string> ComputeAsync(
            string filePath,
            CancellationToken ct = default);

        Task<string> ComputeAsync(
            Stream stream,
            CancellationToken ct = default);

        Task<bool> VerifyAsync(
            string filePath,
            string expectedChecksum,
            CancellationToken ct = default);
    }
}