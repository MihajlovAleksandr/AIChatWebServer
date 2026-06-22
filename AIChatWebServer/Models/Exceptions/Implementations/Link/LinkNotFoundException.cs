using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Link
{
    public class LinkNotFoundException(string token)
        : ApiExceptionBase(404, LinkErrors.LinkNotFound, $"Link with token {token} was not found")
    {
    }
}