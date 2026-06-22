namespace AIChatWebServer.Models.Chats.ValidateSettings
{
    public sealed class MessageSettings(
        bool messageFilesEnabled,
        bool messageImagesEnabled,
        bool voiceMessageEnabled,
        bool videoMessageEnabled)
    {
        public bool MessageFilesEnabled { get; } = messageFilesEnabled;
        public bool MessageImagesEnabled { get; } = messageImagesEnabled;
        public bool VoiceMessageEnabled { get; } = voiceMessageEnabled;
        public bool VideoMessageEnabled { get; } = videoMessageEnabled;

        public static MessageSettings CreateDefault()
        {
            return new MessageSettings(
                true,
                true,
                true,
                true);
        }
    }
}
