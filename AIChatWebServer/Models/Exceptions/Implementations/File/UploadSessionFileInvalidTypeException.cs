using AIChatWebServer.Models.Files;
using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.File
{
    public class UploadSessionFileInvalidTypeException : ApiExceptionBase
    {
        public UploadSessionFileInvalidTypeException(
            Guid uploadSessionFileId,
            FileType expected,
            FileType actual) :
        base(
            400,
            FileErrors.UploadSessionFileInvalidType,
            $"File {uploadSessionFileId} has invalid type. Expected: {expected}, Actual: {actual}")
        {
            
        }

        public UploadSessionFileInvalidTypeException(
            FileType expected,
            FileType actual) :
        base(
            400,
            FileErrors.UploadSessionFileInvalidType,
            $"File has invalid type. Expected: {expected}, Actual: {actual}")
        {

        }
    }
}