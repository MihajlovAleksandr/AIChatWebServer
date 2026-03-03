using AIChatWebServer.Models.Chats.ValidateSettings;

namespace AIChatWebServer.Models.Chats
{
    public class ChatUserData(string name, DateTime joinTime, UserSettings userSettings)
    {
        public string Name { get; private set; } = name;
        public DateTime JoinTime { get; private set; } = joinTime;
        public UserSettings UserSettings { get; private set; } = userSettings;
    }
}
