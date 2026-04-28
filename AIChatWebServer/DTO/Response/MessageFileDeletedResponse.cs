using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record MessageFileDeletedResponse
    (
        [property: JsonPropertyName("messageId")] Guid MessageId,
        [property: JsonPropertyName("fileId")] Guid FileId
    );
}
