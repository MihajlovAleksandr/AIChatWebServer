namespace AIChatWebServer.Models.AI
{
    public class AIMessageRole
    {
        public static readonly AIMessageRole System
            = new AIMessageRole("system");
        public static readonly AIMessageRole User 
            = new AIMessageRole("user");
        public static readonly AIMessageRole Assistant
            = new AIMessageRole("assistant");

        private readonly string role;

        private AIMessageRole(string role)
        {
            this.role = role;  
        }

        public override int GetHashCode() => HashCode.Combine(role);

        public override bool Equals(object? obj)
        {
            if (obj is not AIMessageRole role) return false;
            return role.Equals(role);
        }

        public override string ToString()
        {
            return role;
        }

        public static AIMessageRole? ValueFromString(string str)
        {
            return str?.ToLowerInvariant() switch
            {
                "user" => User,
                "assistant" => Assistant,
                "system" => System,
                _ => null
            };
        }
    }
}
