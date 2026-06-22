using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Connection
{
    public sealed class ConnectionAccessDeniedException(
        Guid connectionId,
        Guid requestedUserId,
        Guid actualUserId) : ApiExceptionBase(
            403,
            SessionErrors.ConnectionForbidden,
            $"Access to connection {connectionId} denied. " +
                $"Requested by user {requestedUserId}, owned by user {actualUserId}")
    {
    }
}
