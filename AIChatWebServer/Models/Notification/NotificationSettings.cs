namespace AIChatWebServer.Models.Notification
{
    public sealed class NotificationSettings(bool emailNotificationsEnabled)
    {
        public bool EmailNotificationsEnabled { get; } = emailNotificationsEnabled;
    }
}
