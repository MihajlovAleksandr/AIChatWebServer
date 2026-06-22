using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public record SearchUserRequest
    {
        [JsonPropertyName("chatMatchPredicate")]
        public required string ChatMatchPredicate { get; init; }
        [JsonPropertyName("chatId")]
        public required Guid ChatId { get; init; }
        [JsonPropertyName("slots")]
        public int Slots { get; init; } = 1;
    }
}
