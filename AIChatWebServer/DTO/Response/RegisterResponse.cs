using AIChatWebServer.Models.User;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public record RegisterResponse
    (
        [property: JsonPropertyName("state")] RegistrationState State,
        [property: JsonPropertyName("token")] string Token
    );
}
