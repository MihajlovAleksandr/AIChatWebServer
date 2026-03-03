namespace AIChatWebServer.Models.Chats.ValidateSettings
{
    public sealed class ChatSettings
    {
        public MemberSettings Members { get; }
        public CallSettings Calls { get; }

        public ChatSettings(
            MemberSettings members,
            CallSettings calls)
        {
            Members = members ?? throw new ArgumentNullException(nameof(members));
            Calls = calls ?? throw new ArgumentNullException(nameof(calls));
        }

        public static ChatSettings CreateDefault()
        {
            return new ChatSettings(
                MemberSettings.CreateDefault(),
                CallSettings.CreateDefault());
        }
    }
}