using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public record OnlineStatusChangedResponse
    (
        [property: JsonPropertyName("userId")] Guid UserId,
        [property: JsonPropertyName("isOnline")] DateTime? LastOnline
    );
}
