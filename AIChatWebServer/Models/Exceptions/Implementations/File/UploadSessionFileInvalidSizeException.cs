using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.File
{
    public class UploadSessionFileInvalidSizeException(
        Guid uploadSessionFileId,
        long expected,
        long actual) :
        ApiExceptionBase(
            400,
            FileErrors.UploadSessionFileInvalidSize,
            $"File {uploadSessionFileId} has invalid size. Expected: {expected}, Actual: {actual}")
    { }
}