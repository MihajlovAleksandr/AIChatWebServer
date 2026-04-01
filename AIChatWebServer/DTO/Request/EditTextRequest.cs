using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public sealed record EditTextRequest
    {
        [JsonPropertyName("text")]
        public required string Text { get; init; }
    }
}
