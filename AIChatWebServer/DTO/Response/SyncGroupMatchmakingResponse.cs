using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record SyncGroupMatchmakingResponse
    (
        [property: JsonPropertyName("isSearching")] bool IsSearching,
        [property: JsonPropertyName("chatId")] Guid? ChatId
    );
}