using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public record ConnectionChangedResponse
    (
        [property: JsonPropertyName("connections")] IReadOnlyCollection<ConnectionResponse> Connections 
    );
}
