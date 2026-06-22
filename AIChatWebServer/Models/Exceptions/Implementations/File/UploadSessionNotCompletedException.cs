using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.File
{
    public class UploadSessionNotCompletedException(Guid uploadSessionId) :
        ApiExceptionBase(
            409,
            FileErrors.UploadSessionNotCompleted,
            $"UploadSession {uploadSessionId} is not completed yet")
    { }

}
