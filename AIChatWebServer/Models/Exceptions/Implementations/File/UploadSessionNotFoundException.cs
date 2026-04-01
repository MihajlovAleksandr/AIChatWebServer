using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.File
{
    public class UploadSessionNotFoundException(Guid uploadSessionId) :
        ApiExceptionBase(404, FileErrors.UploadSessionNotFound, $"UploadSession {uploadSessionId} was not found");
}
