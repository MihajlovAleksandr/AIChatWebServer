using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public sealed record GoogleTokenRequest
    {
        [JsonPropertyName("token")]
        public required string Token { get; init; }
    };
}
