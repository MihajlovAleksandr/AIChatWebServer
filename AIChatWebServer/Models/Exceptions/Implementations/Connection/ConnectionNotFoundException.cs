using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Connection
{
    public sealed class ConnectionNotFoundException(Guid connectionId) : ApiExceptionBase(
            404,
            SessionErrors.ConnectionNotFound,
            $"Connection {connectionId} was not found")
    {
    }
}
