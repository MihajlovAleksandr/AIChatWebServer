using System.Text.Json.Serialization;

namespace AIChatWebServer.Integrations.Telegram.DTO.Response
{
    public record TelegramContextResponse
    (
        [property: JsonPropertyName("id")] Guid Id,
        [property: JsonPropertyName("tgUserId")] long TgUserId,
        [property: JsonPropertyName("langCode")] string LangCode
    );
}
