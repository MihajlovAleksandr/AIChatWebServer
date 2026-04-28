namespace AIChatWebServer.Models.Chats.RandomChat
{
    [Flags]
    public enum QueryTags
    {
        None = 0,
        Casual = 1 << 0,
        SmallTalk = 1 << 1,
        Personal = 1 << 2,
        Emotional = 1 << 3,
        Knowledge = 1 << 4,
        Technical = 1 << 5,
        Complex = 1 << 6
    }
}