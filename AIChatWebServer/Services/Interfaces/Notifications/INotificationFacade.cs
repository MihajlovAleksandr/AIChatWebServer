using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Notification;

namespace AIChatWebServer.Services.Interfaces.Notifications
{
    public interface INotificationFacade
    {
        Task SendToChatAsync(Chat chat, NotificationPrompt prompt, CancellationToken ct = default);
        Task SendToChatAsync(Chat chat, string body, CancellationToken ct = default);
        Task SendToChatAsync(Chat chat, Guid excludedUserId, string body, CancellationToken ct = default);
        Task SendToChatAsync(Chat chat, Guid excludedUserId, NotificationPrompt prompt, CancellationToken ct = default);
        Task SendToUserAsync(Guid chatId, Guid userId, string title, string body, CancellationToken ct = default);
        Task SendToUserAsync(Guid chatId, Guid userId, string title, NotificationPrompt prompt, CancellationToken ct = default);
    }
}
