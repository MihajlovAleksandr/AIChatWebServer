using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public sealed record NotificationTokenRequest
    {
        [JsonPropertyName("token")]
        public required string Token { get; init; }
    }
}
