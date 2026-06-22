using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public sealed record ChatNameRequest
    {
        [JsonPropertyName("name")]
        public required string Name { get; init; }
    }
}
