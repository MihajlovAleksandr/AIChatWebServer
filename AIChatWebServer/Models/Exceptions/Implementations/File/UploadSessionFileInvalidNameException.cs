using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.File
{
    public class UploadSessionFileInvalidNameException(
        Guid uploadSessionFileId,
        string expected,
        string actual) :
        ApiExceptionBase(
            400,
            FileErrors.UploadSessionFileInvalidName,
            $"File {uploadSessionFileId} has invalid name. Expected: '{expected}', Actual: '{actual}'")
    { }
}