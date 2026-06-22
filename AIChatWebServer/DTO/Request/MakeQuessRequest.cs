using AIChatWebServer.Models.Chats.RandomChat;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public class MakeQuessRequest
    {
        [JsonPropertyName("chatId")]
        public required Guid ChatId { get; init; }
        [JsonPropertyName("aiRole")]
        public required AiRole AiRole { get; init; }
    }
}
