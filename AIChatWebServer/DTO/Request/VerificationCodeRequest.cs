using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public sealed record VerificationCodeRequest
    {
        [JsonPropertyName("code")]
        public required string Code { get; init; }
    }
}
