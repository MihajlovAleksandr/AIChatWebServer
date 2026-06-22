using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record ConnectionResponse(
        [property: JsonPropertyName("id")] Guid Id,
        [property: JsonPropertyName("userId")] Guid UserId,
        [property: JsonPropertyName("device")] string Device,
        [property: JsonPropertyName("lastOnline")] DateTime? LastOnline
    );
}
