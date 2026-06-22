using AIChatWebServer.Models.User;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public sealed record UserDataRequest
    {
        [JsonPropertyName("gender")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required Gender Gender { get; init; }
        [JsonPropertyName("age")]
        public required int Age { get; init; }
        [JsonPropertyName("name")]
        public required string Name { get; init; }
    }
}