using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public sealed record CreateThemeRequest
    {
        [JsonPropertyName("name")]
        public required string Name { get; init; }
        [JsonPropertyName("content")]
        public required JsonDocument Content { get; init; }
        [JsonPropertyName("uploadSessionId")]
        public Guid? UploadSessionId { get; init; } = null;
    }
}
