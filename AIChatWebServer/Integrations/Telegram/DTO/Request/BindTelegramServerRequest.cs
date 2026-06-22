using System.Text.Json.Serialization;

namespace AIChatWebServer.Integrations.Telegram.DTO.Request
{
    public sealed record BindTelegramServerRequest
    {
        [JsonPropertyName("tgUserId")]
        public required long TgUserId { get; init; }
        [JsonPropertyName("token")]
        public required string Token { get; init; }
        [JsonPropertyName("language")]
        public required string Language { get; init; }
    }
}
