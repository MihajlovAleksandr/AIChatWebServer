using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public sealed record UpdateAuthIdentitySecretRequest
    {
        [JsonPropertyName("currentProviderCode")]
        public required string CurrentProviderCode { get; init; }
        [JsonPropertyName("currentSecret")]
        public required string CurrentSecret { get; init; }
        [JsonPropertyName("newProviderCode")]
        public string? NewProviderCode { get; init; } = null;
        [JsonPropertyName("newSecret")]
        public required string NewSecret { get; init; }
    }
}
