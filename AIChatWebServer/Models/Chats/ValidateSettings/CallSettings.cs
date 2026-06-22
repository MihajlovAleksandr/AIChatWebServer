namespace AIChatWebServer.Models.Chats.ValidateSettings
{
    public sealed class CallSettings
    {
        public bool CallEnabled { get; }
        public bool VideoEnabled { get; }

        public CallSettings(bool callEnabled, bool videoEnabled)
        {
            CallEnabled = callEnabled;
            VideoEnabled = videoEnabled;
        }

        public static CallSettings CreateDefault()
        {
            return new CallSettings(
                callEnabled: true,
                videoEnabled: true);
        }
    }
}