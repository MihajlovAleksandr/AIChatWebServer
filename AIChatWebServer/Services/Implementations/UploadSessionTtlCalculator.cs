using AIChatWebServer.Models.Files;
using AIChatWebServer.Services.Interfaces;

namespace AIChatWebServer.Services.Implementations
{
    public sealed class UploadSessionTtlCalculator : IUploadSessionTtlCalculator
    {
        private const long ExpectedUploadSpeedBytesPerSec = 512 * 1024;
        private const int OverheadSeconds = 60;
        private const int MinTtlSeconds = 60;
        private const int MaxTtlSeconds = 3600;

        public DateTime CalculateExpiration(
            IReadOnlyCollection<UploadSessionFile> files,
            DateTime now)
        {
            if (files == null || files.Count == 0)
                return now.AddSeconds(MinTtlSeconds);

            var totalSize = files.Sum(f => f.ExpectedFileSize);

            var uploadSeconds = totalSize / (double)ExpectedUploadSpeedBytesPerSec;

            var ttlSeconds = uploadSeconds + OverheadSeconds;

            ttlSeconds = Math.Max(ttlSeconds, MinTtlSeconds);
            ttlSeconds = Math.Min(ttlSeconds, MaxTtlSeconds);

            return now.AddSeconds(ttlSeconds);
        }
    }
}
