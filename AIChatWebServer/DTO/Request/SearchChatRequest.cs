using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public record SearchChatRequest
    {
        [JsonPropertyName("chatMatchPredicate")]
        public required string ChatMatchPredicate { get; init; }
        [JsonPropertyName("chatName")]
        public required string ChatName { get; init; }
    }
}
