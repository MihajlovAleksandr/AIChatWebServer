using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record ChatEndedResponse
    (
        [property: JsonPropertyName("chatId")] Guid ChatId,
        [property: JsonPropertyName("endedTime")] DateTime EndedTime
    );
}
