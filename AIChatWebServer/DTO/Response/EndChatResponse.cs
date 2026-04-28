using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record EndChatResponse
    (
        [property: JsonPropertyName("endedTime")] DateTime EndedTime
    );
}
