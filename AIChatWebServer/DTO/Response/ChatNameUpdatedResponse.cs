using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record ChatNameUpdatedResponse
    (
        [property: JsonPropertyName("chatId")] Guid ChatId, 
        [property: JsonPropertyName("name")] string Name
    );
    
}
