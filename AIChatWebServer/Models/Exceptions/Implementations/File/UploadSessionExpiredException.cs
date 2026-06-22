using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.File
{
    public class UploadSessionExpiredException(Guid uploadSessionId) :
        ApiExceptionBase(
            410,
            FileErrors.UploadSessionExpired,
            $"UploadSession {uploadSessionId} is expired")
    { }
}
