using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Notification
{
    public sealed class NotificationTokenNotFoundException(Guid connectionId) : ApiExceptionBase(
            404,
            NotificationErrors.NotificationTokenNotFound,
            $"Notification token for connection '{connectionId}' was not found")
    {
    }
}
