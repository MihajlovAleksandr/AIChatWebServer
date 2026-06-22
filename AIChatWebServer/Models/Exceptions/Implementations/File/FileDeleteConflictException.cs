namespace AIChatWebServer.Models.Exceptions.Implementations.File
{
    public class FileDeleteConflictException(Guid fileId)
        : ApiExceptionBase(
            409,
            Utils.Errors.FileErrors.FileDeleteConflict,
            $"File {fileId} cannot be deleted because it is in use or already deleted")
    {
    }
}