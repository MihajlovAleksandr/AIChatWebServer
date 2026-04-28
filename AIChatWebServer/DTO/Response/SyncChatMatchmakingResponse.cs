using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record SyncChatMatchmakingResponse
    (
        [property: JsonPropertyName("isSearching")] bool IsSearching
    );
}