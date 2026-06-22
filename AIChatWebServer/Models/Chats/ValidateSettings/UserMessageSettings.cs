namespace AIChatWebServer.Models.Chats.ValidateSettings
{
    public sealed class UserMessageSettings(
        bool messagesEnabled,
        bool messageFilesEnabled,
        bool messageImagesEnabled,
        bool voiceMessageEnabled,
        bool videoMessageEnabled,
        bool editMessagesEnabled,
        bool deleteOwnMessagesEnabled,
        bool deleteOtherMessagesEnabled)
    {
        public bool MessagesEnabled { get; } = messagesEnabled;
        public bool MessageFilesEnabled { get; } = messageFilesEnabled;
        public bool MessageImagesEnabled { get; } = messageImagesEnabled;
        public bool VoiceMessageEnabled { get; } = voiceMessageEnabled;
        public bool VideoMessageEnabled { get; } = videoMessageEnabled;
        public bool EditMessagesEnabled { get; } = editMessagesEnabled;
        public bool DeleteOwnMessagesEnabled { get; } = deleteOwnMessagesEnabled;
        public bool DeleteOtherMessagesEnabled { get; } = deleteOtherMessagesEnabled;

        public static UserMessageSettings CreateDefault()
        {
            return new UserMessageSettings(true, true, true, true, true, true, true, false);
        }

        public static UserMessageSettings CreateFullAccess()
        {
            return new UserMessageSettings(true, true, true, true, true, true, true, true);
        }
    }
}