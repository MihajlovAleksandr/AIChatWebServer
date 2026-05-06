using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record ProductResponse
        (
            [property: JsonPropertyName("id")] Guid Id,
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("price")] decimal Price,
            [property: JsonPropertyName("currency")] string Currency,
            [property: JsonPropertyName("description")] string Description
        );
}
