using AIChatWebServer.Models.Links;
using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Link
{
    public class LinkAlreadyExistsForTypeException(Guid userId, LinkType type)
        : ApiExceptionBase(409, LinkErrors.LinkAlreadyExistsForType, $"User {userId} already has an active link of type {type}")
    {
    }
}