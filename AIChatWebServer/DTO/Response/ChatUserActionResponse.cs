using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record ChatUserActionResponse
    (
        [property: JsonPropertyName("chatId")] Guid ChatId,
        [property: JsonPropertyName("userId")] Guid UserId
    );
}
