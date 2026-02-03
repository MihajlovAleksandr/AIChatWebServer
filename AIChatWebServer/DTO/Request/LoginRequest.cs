using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public record LoginRequest
    (
        [property: JsonPropertyName("identifier")] string Identifier,
        [property: JsonPropertyName("secret")] string Secret, 
        [property: JsonPropertyName("identityProviderCode")] string IdentityProviderCode
    );
}
