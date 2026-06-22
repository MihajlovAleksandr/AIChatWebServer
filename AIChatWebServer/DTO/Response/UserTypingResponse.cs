using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record UserTypingResponse
    (
        [property: JsonPropertyName("userId")] Guid UserId,
        [property: JsonPropertyName("chatId")] Guid ChatId,
        [property: JsonPropertyName("isTyping")] bool IsTyping
    );
}
