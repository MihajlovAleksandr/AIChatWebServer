using AIChatWebServer.Models.Files;
using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.File
{
    public class UploadSessionFileInvalidTypeException(
        Guid uploadSessionFileId,
        FileType expected,
        FileType actual) :
        ApiExceptionBase(
            400,
            FileErrors.UploadSessionFileInvalidType,
            $"File {uploadSessionFileId} has invalid type. Expected: {expected}, Actual: {actual}")
    { }
}