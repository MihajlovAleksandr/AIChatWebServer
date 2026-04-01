using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.File
{
    public class UploadSessionEntityAlreadyAssignedException(Guid uploadSessionId, Guid entityId) :
        ApiExceptionBase(
            409,
            FileErrors.UploadSessionEntityAlreadyAssigned,
            $"UploadSession {uploadSessionId} is already assigned to entity {entityId}")
    { }
}