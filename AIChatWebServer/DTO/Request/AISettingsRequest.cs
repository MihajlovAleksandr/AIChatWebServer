using AIChatWebServer.Models.AI;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public sealed record AISettingsRequest
    {
        [JsonPropertyName("prompt")]
        public string? Prompt { get; init; } = null;
        [JsonPropertyName("aiModel")]
        public required AIModel AIModel { get; init; }
    }
}
