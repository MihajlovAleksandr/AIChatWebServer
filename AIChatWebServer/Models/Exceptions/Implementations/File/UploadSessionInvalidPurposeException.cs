using AIChatWebServer.Utils.Errors;
using AIChatWebServer.Models.Files;

namespace AIChatWebServer.Models.Exceptions.Implementations.File
{
    public class UploadSessionInvalidPurposeException(Guid uploadSessionId, UploadSessionPurpose actualPurpose) :
        ApiExceptionBase(
            409,
            FileErrors.UploadSessionInvalidPurpose,
            $"UploadSession {uploadSessionId} has invalid purpose {actualPurpose}")
    { }
}