using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public record MessageDeletedResponse
    (
        [property: JsonPropertyName("messageId")] Guid MessageId
    );
    
}
