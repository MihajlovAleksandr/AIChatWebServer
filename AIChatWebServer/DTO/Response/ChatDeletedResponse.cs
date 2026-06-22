using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record ChatDeletedResponse
    (
        [property: JsonPropertyName("chatId")] Guid ChatId
    );
}
