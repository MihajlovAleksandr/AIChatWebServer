using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Connection
{
    public sealed class InvalidConnectionDeviceException(
        Guid connectionId,
        string requestedDevice,
        string actualDevice) : ApiExceptionBase(
            403,
            SessionErrors.InvalidConnection,
            $"Invalid device for connection {connectionId}. " +
                $"Requested: {requestedDevice}, Actual: {actualDevice}")
    {
    }
}
