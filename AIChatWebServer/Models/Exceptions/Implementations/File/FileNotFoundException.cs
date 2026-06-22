using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.File
{
    public class FileNotFoundException(Guid fileId) : ApiExceptionBase(404, FileErrors.FileNotFound, $"File {fileId} was not found")
    {
    }
}
