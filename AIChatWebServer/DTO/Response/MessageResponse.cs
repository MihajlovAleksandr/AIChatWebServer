using AIChatWebServer.Models.Messages;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record MessageResponse
    (
        [property: JsonPropertyName("id")] Guid Id,
        [property: JsonPropertyName("chatId")] Guid ChatId,
        [property: JsonPropertyName("userId")] Guid UserId,
        [property: JsonPropertyName("text")] string Text,
        [property: JsonPropertyName("time")] DateTime Time,
        [property: JsonPropertyName("lastUpdate")] DateTime LastUpdate,
        [property: JsonPropertyName("replies")] IReadOnlyCollection<MessageReplyResponse> Replies,
        [property: JsonPropertyName("statuses")] IReadOnlyDictionary<Guid, MessageStatus> Statuses,
        [property: JsonPropertyName("files")] IReadOnlyCollection<Guid> Files
    );
}
