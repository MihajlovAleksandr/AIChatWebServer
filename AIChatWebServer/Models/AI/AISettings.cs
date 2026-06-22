namespace AIChatWebServer.Models.AI
{
    public class AISettings
    {
        public AIMessageBuffers AIMessageBuffers { get; set; } = new();
    }

    public class AIMessageBuffers
    {
        public MessageRange Messages { get; set; } = new();
        public MessageRange CompressedMessages { get; set; } = new();
    }

    public class MessageRange
    {
        public int Min { get; set; }
        public int Max { get; set; }
    }
}