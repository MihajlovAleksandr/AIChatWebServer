using AIChatWebServer.Models.Notification;
using AIChatWebServer.Services.Interfaces.Notifications;

namespace AIChatWebServer.Services.Implementations.Notifications
{
    public class NotificationSender(INotificationTokenGetter tokenGetter,
        IMessageNotificationService messageNotificationService) : INotificationSender
    {
        private readonly INotificationTokenGetter _tokenGetter = tokenGetter;
        private readonly IMessageNotificationService _messageNotificationService = messageNotificationService;

        public async Task SendAsync(IEnumerable<Guid> users, Guid chatId, string title, string body, CancellationToken ct)
        {
            await SendNotificationAsync(users, chatId, title, body, false, ct);
        }

        public async Task SendAsync(IEnumerable<Guid> users, Guid chatId, string title, NotificationPrompt prompt, CancellationToken ct)
        {
            await SendNotificationAsync(users, chatId, title, prompt.ToString(), true, ct);
        }

        private async Task SendNotificationAsync(IEnumerable<Guid> users, Guid chatId, string title, string body, bool isBodyPrompt, CancellationToken ct)
        {
            var tokens = await _tokenGetter.GetNotificationTokensAsync([.. users], ct);
            foreach(var userTokens in tokens.Values) 
                foreach(var token in userTokens) 
                    await _messageNotificationService.SendMessageToDevice(token, title, body, chatId, isBodyPrompt);
        }
    }
}
