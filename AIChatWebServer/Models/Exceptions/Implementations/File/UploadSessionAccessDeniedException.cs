using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.File
{
    public class UploadSessionAccessDeniedException(Guid uploadSessionId, Guid userId) :
        ApiExceptionBase(
            403,
            FileErrors.UploadSessionAccessDenied,
            $"UploadSession {uploadSessionId} does not belong to user {userId}")
    { }
}