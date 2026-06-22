using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public sealed record UpdateThemeRequest
    {
        [JsonPropertyName("name")]
        public required string Name { get; init; }
        [JsonPropertyName("content")]
        public required JsonDocument Content { get; init; }
    }
}
