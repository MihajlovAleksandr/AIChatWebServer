namespace AIChatWebServer.Utils.Errors
{
    public sealed class NotificationErrors : ErrorCode
    {
        private NotificationErrors(string code) : base(code)
        {
        }

        public static readonly IErrorCode NotificationSettingsNotFound =
            new NotificationErrors("NOTIFICATION_SETTINGS_NOT_FOUND");

        public static readonly IErrorCode NotificationTokenNotFound =
            new NotificationErrors("NOTIFICATION_TOKEN_NOT_FOUND");
    }
}
