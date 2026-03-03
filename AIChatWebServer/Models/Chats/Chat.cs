using AIChatWebServer.Models.Chats.ValidateSettings;

namespace AIChatWebServer.Models.Chats
{
    public class Chat
    {
        public Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? EndTime { get; set; }
        public ChatType Type { get; set; }
        public ChatSettings Settings { get; set; }
        public Dictionary<Guid, ChatUserData> UsersWithData { get; set; }

        public Chat()
        {
            UsersWithData = new Dictionary<Guid, ChatUserData>();
            CreationTime = DateTime.Now;
            Settings = ChatSettings.CreateDefault();
        }

        public bool ContainsAI(Guid aiId)
        {
            return UsersWithData.TryGetValue(aiId, out ChatUserData? _);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Chat other) return false;
            return Id.Equals(other.Id);
        }

        public override string ToString()
        {
            return $"Chat #{Id}\n{CreationTime} - {EndTime}\n";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
