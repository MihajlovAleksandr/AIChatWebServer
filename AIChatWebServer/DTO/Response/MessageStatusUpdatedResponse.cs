using AIChatWebServer.Models.Messages;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record MessageStatusUpdatedResponse
    (
        [property: JsonPropertyName("messageId")] IReadOnlyCollection<Guid> MessageIds,
        [property: JsonPropertyName("userId")] Guid UserId,
        [property: JsonPropertyName("status")] MessageStatus Status
    );
}
