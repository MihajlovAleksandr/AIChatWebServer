using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Notification
{
    public sealed class NotificationSettingsNotFoundException(Guid userId) : ApiExceptionBase(
            404,
            NotificationErrors.NotificationSettingsNotFound,
            $"Notification settings for user '{userId}' were not found")
    {
    }
}
