using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.File
{
    public class UploadSessionAlreadyCompletedException(Guid uploadSessionId) :
        ApiExceptionBase(
            409,
            FileErrors.UploadSessionAlreadyCompleted,
            $"UploadSession {uploadSessionId} is already completed")
    { }
}
