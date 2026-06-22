using AIChatWebServer.Models.Files;

namespace AIChatWebServer.Services.Interfaces.Files
{
    public interface IUploadSessionTtlCalculator
    {
        DateTime CalculateExpiration(
            IReadOnlyCollection<UploadSessionFile> files,
            DateTime now);
    }
}
