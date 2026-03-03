using AIChatWebServer.Models.Chats;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record ChatResponse
    (
        [property: JsonPropertyName("id")] Guid Id,
        [property: JsonPropertyName("type")] ChatType ChatType,
        [property: JsonPropertyName("endTime")] DateTime? EndTime,
        [property: JsonPropertyName("users")] IEnumerable<Guid> Users,
        [property: JsonPropertyName("name")] string Name
    );
}
