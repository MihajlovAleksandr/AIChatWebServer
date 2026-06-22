using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record MessageUpdatedResponse
    (
        [property: JsonPropertyName("messageId")] Guid MessageId,
        [property: JsonPropertyName("text")] string Text,
        [property: JsonPropertyName("lastUpdate")] DateTime LastUpdate
    );
    
}
