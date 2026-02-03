using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public record VerificationCodeRequest
    (
        [property: JsonPropertyName("code")] string Code
    );
}
