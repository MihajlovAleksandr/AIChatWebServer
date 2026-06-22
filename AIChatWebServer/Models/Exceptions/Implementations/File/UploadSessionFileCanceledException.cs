using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.File
{
    public class UploadSessionFileCanceledException(Guid fileId) :
        ApiExceptionBase(
            409,
            FileErrors.UploadSessionFileCanceled,
            $"File {fileId} upload was canceled")
    { }
}
