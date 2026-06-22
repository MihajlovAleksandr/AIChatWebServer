using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public sealed record UserPremiumRequest
    {
        [JsonPropertyName("id")]
        public required Guid Id { get; init; }
        [JsonPropertyName("startTime")]
        public required DateTime StartTime { get; init; }
        [JsonPropertyName("endTime")]
        public required DateTime EndTime { get; init; }
    }
}
