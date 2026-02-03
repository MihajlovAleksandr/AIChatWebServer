using AIChatWebServer.Models.User;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public record PreferenceRequest
    (
        [property: JsonPropertyName("minAge")] int MinAge,
        [property: JsonPropertyName("maxAge")] int MaxAge,
        [property: JsonPropertyName("gender")] PreferenceGender Gender
    );
}
