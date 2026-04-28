using AIChatWebServer.Models.Notification;

namespace AIChatWebServer.Services.Interfaces.Notifications
{
    public interface INotificationService
    {
        Task UpdateSettingsAsync(
            Guid userId,
            NotificationSettings settings,
            CancellationToken cancellationToken = default);

        Task<NotificationSettings> GetSettingsAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task UpdateNotificationTokenAsync(
            Guid connectionId,
            string token,
            CancellationToken cancellationToken = default);
    }
}
