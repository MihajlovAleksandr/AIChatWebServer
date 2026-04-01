using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.File
{
    public class UploadSessionFileFailedException(Guid fileId) :
        ApiExceptionBase(
            409,
            FileErrors.UploadSessionFileFailed,
            $"File {fileId} upload failed")
    { }
}
