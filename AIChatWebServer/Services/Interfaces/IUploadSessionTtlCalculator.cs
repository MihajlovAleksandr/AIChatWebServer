using AIChatWebServer.Models.Files;

namespace AIChatWebServer.Services.Interfaces
{
    public interface IUploadSessionTtlCalculator
    {
        DateTime CalculateExpiration(
            IReadOnlyCollection<UploadSessionFile> files,
            DateTime now);
    }
}
