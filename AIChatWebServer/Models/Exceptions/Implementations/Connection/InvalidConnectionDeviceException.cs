using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Connection
{
    public sealed class InvalidConnectionDeviceException : ApiExceptionBase
    {
        public Guid ConnectionId { get; }

        public string RequestedDevice { get; }

        public string ActualDevice { get; }

        public InvalidConnectionDeviceException(
            Guid connectionId,
            string requestedDevice,
            string actualDevice)
            : base(
                403,
                SessionErrors.InvalidConnection,
                $"Invalid device for connection {connectionId}. " +
                $"Requested: {requestedDevice}, Actual: {actualDevice}")
        {
            ConnectionId = connectionId;
            RequestedDevice = requestedDevice;
            ActualDevice = actualDevice;
        }
    }
}
