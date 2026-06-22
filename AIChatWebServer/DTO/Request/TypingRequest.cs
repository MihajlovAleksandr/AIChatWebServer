using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public class TypingRequest
    {
        [JsonPropertyName("chatId")]
        public required Guid ChatId { get; init; }
        [JsonPropertyName("isTyping")]
        public required bool IsTyping { get; init; }
    }
}
