using AIChatWebServer.Models.Chats.ValidateSettings;

namespace AIChatWebServer.Models.Chats
{
    public class ChatUserData(Guid id, string name, DateTime joinTime, UserSettings userSettings)
    {
        public Guid Id { get; private set; } = id;
        public string Name { get; private set; } = name;
        public DateTime JoinTime { get; private set; } = joinTime;
        public UserSettings UserSettings { get; private set; } = userSettings;
    }
}
