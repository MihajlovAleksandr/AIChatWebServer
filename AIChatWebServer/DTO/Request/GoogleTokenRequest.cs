using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public record GoogleTokenRequest(
        [property: JsonPropertyName("token")] string Token
    );
}
