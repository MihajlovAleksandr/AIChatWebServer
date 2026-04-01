using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.File
{
    public class FileDoesNotBelongToSessionException(Guid fileId, Guid uploadSessionId) :
        ApiExceptionBase(
            400,
            FileErrors.FileDoesNotBelongToSession,
            $"File {fileId} does not belong to upload session {uploadSessionId}")
    {
    }
}
