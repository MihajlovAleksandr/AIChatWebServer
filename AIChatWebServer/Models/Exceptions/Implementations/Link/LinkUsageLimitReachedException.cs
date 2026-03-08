using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Link
{
    public class LinkUsageLimitReachedException(Guid linkId)
        : ApiExceptionBase(400, LinkErrors.LinkUsageLimitReached, $"Usage limit reached for link {linkId}")
    {
    }
}