namespace AIChatWebServer.Models.AI
{
    public class AIMessage
    {
        public AIMessage(Guid id, Guid chatId, AIMessageRole role, AIMessageType type, string content)
        {
            Id = id;
            ChatId = chatId;
            Role = role;
            Type = type;
            Content = content;

        }

        public AIMessage(Guid chatId, AIMessageRole role, AIMessageType type, string content)
        {
            Id = Guid.NewGuid();
            ChatId = chatId;
            Role = role;
            Type = type;
            Content = content;
        }

        public Guid Id { get; set; }
        public Guid ChatId { get; set; }
        public AIMessageRole Role { get; set; }
        public AIMessageType Type { get; set; }
        public string Content { get; set; }
    }
}