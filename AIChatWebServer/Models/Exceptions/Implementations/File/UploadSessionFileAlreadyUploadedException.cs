using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.File
{
    public class UploadSessionFileAlreadyUploadedException(Guid fileId) :
        ApiExceptionBase(
            409,
            FileErrors.UploadSessionFileAlreadyUploaded,
            $"File {fileId} is already uploaded")
    { }
}
