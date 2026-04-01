using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.File
{
    public class UploadSessionFilesNotFullyUploadedException(Guid uploadSessionId) :
        ApiExceptionBase(
            409,
            FileErrors.UploadSessionFilesNotFullyUploaded,
            $"UploadSession {uploadSessionId} cannot be completed because not all files are uploaded")
    { }
}