using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record SyncResponse
    (
        [property: JsonPropertyName("matchmaking")] SyncMatchmakingResponse Matchmaking,
        [property: JsonPropertyName("messages")] SyncMessagesResponse Messages,
        [property: JsonPropertyName("chats")] SyncChatsResponse Chats
    );
}