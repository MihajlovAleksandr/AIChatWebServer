namespace AIChatWebServer.Models.Chats.ValidateSettings
{
    public sealed class MemberSettings
    {
        public bool AllowAddByLink { get; }
        public bool AllowSearchJoin { get; }

        public MemberSettings(
            bool allowAddByLink,
            bool allowSearchJoin)
        {
            AllowAddByLink = allowAddByLink;
            AllowSearchJoin = allowSearchJoin;
        }

        public static MemberSettings CreateDefault()
        {
            return new MemberSettings(
                allowAddByLink: true,
                allowSearchJoin: true);
        }
    }
}