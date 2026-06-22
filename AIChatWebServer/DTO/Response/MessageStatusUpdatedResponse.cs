using AIChatWebServer.Models.Messages;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record MessageStatusUpdatedResponse
    (
        [property: JsonPropertyName("messageIds")] IReadOnlyCollection<Guid> MessageIds,
        [property: JsonPropertyName("userId")] Guid UserId,
        [property: JsonPropertyName("chatId")] Guid ChatId,
        [property: JsonPropertyName("status")] MessageStatus Status
    );
}
