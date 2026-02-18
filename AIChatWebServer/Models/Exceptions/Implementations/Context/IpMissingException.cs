using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Context
{
    public sealed class IpMissingException() : ApiExceptionBase(400, CommonErrors.IpMissing, "Ip is missing")
    {
    }
}
