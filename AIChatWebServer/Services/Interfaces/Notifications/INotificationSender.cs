using AIChatWebServer.Models.Notification;

namespace AIChatWebServer.Services.Interfaces.Notifications
{
    public interface INotificationSender
    {
        Task SendAsync(IEnumerable<Guid> users, Guid chatId, string title, string body, CancellationToken ct);
        Task SendAsync(IEnumerable<Guid> users, Guid chatId, string title, NotificationPrompt prompt, CancellationToken ct);
    }
}
