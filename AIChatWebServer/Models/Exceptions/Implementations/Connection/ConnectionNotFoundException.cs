using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Connection
{
    public sealed class ConnectionNotFoundException : ApiExceptionBase
    {
        public Guid ConnectionId { get; }

        public ConnectionNotFoundException(Guid connectionId)
            : base(
                404,
                SessionErrors.ConnectionNotFound,
                $"Connection {connectionId} was not found")
        {
            ConnectionId = connectionId;
        }
    }
}
