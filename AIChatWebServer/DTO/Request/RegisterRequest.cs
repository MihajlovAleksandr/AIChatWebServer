using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public sealed record RegisterRequest
    {
        [JsonPropertyName("identifier")]
        public required string Identifier { get; init; }
        [JsonPropertyName("secret")] 
        public required string Secret { get; init; }
        [JsonPropertyName("identityProviderCode")] 
        public required string IdentityProviderCode { get; init; }
    }
}
