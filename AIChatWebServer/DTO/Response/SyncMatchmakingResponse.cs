using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record SyncMatchmakingResponse
    (
        [property: JsonPropertyName("chatMatchmaking")] SyncChatMatchmakingResponse ChatMatchmaking,
        [property: JsonPropertyName("groupMatchmaking")] SyncGroupMatchmakingResponse GroupMatchmaking
    );
}