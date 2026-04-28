using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record ChatSeachingStatusResponse
    (
        [property: JsonPropertyName("isSearching")] bool IsSearching
    );
}
