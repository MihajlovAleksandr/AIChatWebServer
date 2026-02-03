using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public sealed record UserRequest
    (
        [property: JsonPropertyName("userId")] int UserId,
        [property: JsonPropertyName("email")] string Email
    );
}
