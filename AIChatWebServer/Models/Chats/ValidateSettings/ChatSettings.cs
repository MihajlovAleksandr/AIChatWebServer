namespace AIChatWebServer.Models.Chats.ValidateSettings
{
    public sealed class ChatSettings(
        MemberSettings members,
        CallSettings calls,
        MessageSettings messages)
    {
        public MemberSettings Members { get; } = members 
            ?? throw new ArgumentNullException(nameof(members));
        public CallSettings Calls { get; } = calls 
            ?? throw new ArgumentNullException(nameof(calls));
        public MessageSettings Messages { get; } = messages 
            ?? throw new ArgumentNullException(nameof(messages));

        public static ChatSettings CreateDefault()
        {
            return new ChatSettings(
                MemberSettings.CreateDefault(),
                CallSettings.CreateDefault(),
                MessageSettings.CreateDefault());
        }
    }
}