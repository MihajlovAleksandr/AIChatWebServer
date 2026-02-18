using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Context
{
    public sealed class DeviceMissingException() : ApiExceptionBase(400, CommonErrors.DeviceMissing, "Device identifier is missing")
    {
    }
}
