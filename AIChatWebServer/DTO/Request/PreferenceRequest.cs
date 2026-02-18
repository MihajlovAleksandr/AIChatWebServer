using AIChatWebServer.Models.User;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public sealed record PreferenceRequest
    {
        [JsonPropertyName("minAge")] 
        public required int MinAge { get; init; }
        [JsonPropertyName("maxAge")]
        public required int MaxAge { get; init; }
        [JsonPropertyName("gender")]
        public required PreferenceGender Gender { get; init; }
    };
}
