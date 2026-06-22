using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.File
{
    public class UploadSessionCanceledException(Guid uploadSessionId) :
        ApiExceptionBase(
            409,
            FileErrors.UploadSessionCanceled,
            $"UploadSession {uploadSessionId} is canceled")
    { }

}
