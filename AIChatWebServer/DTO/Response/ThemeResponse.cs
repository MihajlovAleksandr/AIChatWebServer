using AIChatWebServer.Models.Themes;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public record ThemeResponse
    (
        [property: JsonPropertyName("id")] Guid Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("type")] ThemeType Type,
        [property: JsonPropertyName("content")] JsonDocument Content,
        [property: JsonPropertyName("usageCount")] long? UsageCount
    );
}
