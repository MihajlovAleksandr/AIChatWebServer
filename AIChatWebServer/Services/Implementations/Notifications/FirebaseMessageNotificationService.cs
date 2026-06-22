using AIChatWebServer.Services.Interfaces.Notifications;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;

namespace AIChatWebServer.Services.Implementations.Notifications
{
    public class FirebaseMessageNotificationService(
        FirebaseApp firebaseApp,
        ILogger<FirebaseMessageNotificationService> logger) : IMessageNotificationService
    {
        private readonly FirebaseApp _firebaseApp = firebaseApp ?? throw new ArgumentNullException(nameof(firebaseApp));
        private readonly ILogger<FirebaseMessageNotificationService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        private async Task SendMessageToDeviceInternal(string deviceToken, Dictionary<string, string> data)
        {
            var message = new Message()
            {
                Token = deviceToken,
                Data = data,
                Android = new AndroidConfig()
                {
                    Priority = Priority.High
                },
                Apns = new ApnsConfig()
                {
                    Headers = new Dictionary<string, string>
            {
                { "apns-priority", "10" }
            },
                    Aps = new Aps()
                    {
                        ContentAvailable = true
                    }
                }
            };

            try
            {
                var messaging = FirebaseMessaging.GetMessaging(_firebaseApp);

                string response = await messaging.SendAsync(message);

                _logger.LogInformation(
                    "Successfully sent data message to device {DeviceToken}: {Response}",
                    deviceToken,
                    response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error sending data message to device {DeviceToken}.",
                    deviceToken);
            }
        }

        public async Task SendMessageToDevice(string deviceToken, string title, string body, Guid chatId, bool isBodyPrompt)
        {
            var data = new Dictionary<string, string>
            {
                { "title", title },
                { "body", body },
                { "chatId", chatId.ToString() },
                { "isBodyPrompt", isBodyPrompt.ToString() }
            };

            await SendMessageToDeviceInternal(deviceToken, data);
        }
    }
}