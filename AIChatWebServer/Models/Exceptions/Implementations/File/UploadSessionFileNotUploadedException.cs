using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.File
{
    public class UploadSessionFileNotUploadedException(Guid fileId) :
        ApiExceptionBase(
            409,
            FileErrors.UploadSessionFileNotUploaded,
            $"File {fileId} is not uploaded yet")
    { }
}
