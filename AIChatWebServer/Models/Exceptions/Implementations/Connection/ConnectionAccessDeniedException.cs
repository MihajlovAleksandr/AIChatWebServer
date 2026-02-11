using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Connection
{
    public sealed class ConnectionAccessDeniedException : ApiExceptionBase
    {
        public Guid ConnectionId { get; }

        public Guid RequestedUserId { get; }

        public Guid ActualUserId { get; }

        public ConnectionAccessDeniedException(
            Guid connectionId,
            Guid requestedUserId,
            Guid actualUserId)
            : base(
                403,
                SessionErrors.ConnectionForbidden,
                $"Access to connection {connectionId} denied. " +
                $"Requested by user {requestedUserId}, owned by user {actualUserId}")
        {
            ConnectionId = connectionId;
            RequestedUserId = requestedUserId;
            ActualUserId = actualUserId;
        }
    }
}
