using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Notification;
using AIChatWebServer.Services.Interfaces.Notifications;

namespace AIChatWebServer.Services.Implementations.Notifications
{
    public class NotificationFacade(INotificationSender notificationSender) : INotificationFacade
    {
        private readonly INotificationSender _notificationSender = notificationSender;

        public async Task SendToChatAsync(Chat chat, NotificationPrompt prompt, CancellationToken ct = default)
        {
            var tasks = chat.UsersWithData
                .Select(item => _notificationSender.SendAsync(
                    [item.Key],
                    chat.Id,
                    item.Value.Name,
                    prompt,
                    ct));

            await Task.WhenAll(tasks);
        }

        public async Task SendToChatAsync(Chat chat, Guid excludedUserId, string body, CancellationToken ct = default)
        {
            var tasks = chat.UsersWithData
                .Where(item => item.Key != excludedUserId)
                .Select(item => _notificationSender.SendAsync(
                    [item.Key],
                    chat.Id,
                    item.Value.Name,
                    body,
                    ct));

            await Task.WhenAll(tasks);
        }

        public async Task SendToChatAsync(Chat chat, Guid excludedUserId, NotificationPrompt prompt, CancellationToken ct = default)
        {
            var tasks = chat.UsersWithData
                .Where(item => item.Key != excludedUserId)
                .Select(item => _notificationSender.SendAsync(
                    [item.Key],
                    chat.Id,
                    item.Value.Name,
                    prompt,
                    ct));

            await Task.WhenAll(tasks);
        }

        public async Task SendToChatAsync(Chat chat, string body, CancellationToken ct = default)
        {
            var tasks = chat.UsersWithData
                .Select(item => _notificationSender.SendAsync(
                    [item.Key],
                    chat.Id,
                    item.Value.Name,
                    body,
                    ct));

            await Task.WhenAll(tasks);
        }

        public Task SendToUserAsync(Guid chatId, Guid userId, string title, string body, CancellationToken ct = default)
        {
            return _notificationSender.SendAsync(
                [userId],
                chatId,
                title,
                body,
                ct);
        }

        public Task SendToUserAsync(Guid chatId, Guid userId, string title, NotificationPrompt prompt, CancellationToken ct = default)
        {
            return _notificationSender.SendAsync(
                [userId],
                chatId,
                title,
                prompt,
                ct);
        }
    }
}