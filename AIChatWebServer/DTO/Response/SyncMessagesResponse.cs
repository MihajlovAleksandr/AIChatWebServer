using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record SyncMessagesResponse
    (
        [property: JsonPropertyName("newMessages")] IReadOnlyCollection<MessageResponse> NewMessages,
        [property: JsonPropertyName("updatedMessages")] IReadOnlyCollection<MessageResponse> UpdatedMessages,
        [property: JsonPropertyName("deletedMessages")] IReadOnlyCollection<Guid> DeletedMessages
    );
}