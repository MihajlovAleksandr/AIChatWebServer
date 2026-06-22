using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record SyncChatsResponse
    (
        [property: JsonPropertyName("newChats")] IReadOnlyCollection<ChatResponse> NewChats,
        [property: JsonPropertyName("updatedChats")] IReadOnlyCollection<ChatResponse> UpdatedChats,
        [property: JsonPropertyName("deletedChats")] IReadOnlyCollection<Guid> DeletedChats
    );
}