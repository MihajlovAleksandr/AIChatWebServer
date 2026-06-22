using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Link
{
    public class LinkNotActiveException(Guid linkId)
        : ApiExceptionBase(400, LinkErrors.LinkNotActive, $"Link {linkId} is not active")
    {
    }
}