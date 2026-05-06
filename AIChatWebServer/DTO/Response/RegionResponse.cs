using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record RegionResponse
    (
        [property: JsonPropertyName("code")] string Code,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("currency")] string Currency
    );
}
