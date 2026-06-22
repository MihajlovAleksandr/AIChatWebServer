namespace AIChatWebServer.Services.Interfaces.Notifications
{
    public interface IMessageNotificationService
    {
        Task SendMessageToDevice(string deviceToken, string title, string body, Guid chatId, bool isBodyPrompt);
    }
}
