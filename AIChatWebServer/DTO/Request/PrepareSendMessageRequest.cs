using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public record PrepareSendMessageRequest
    {
        [JsonPropertyName("chatId")]
        public required Guid ChatId { get; init; }

        [JsonPropertyName("textLength")]
        public required int TextLength { get; init; }

        [JsonPropertyName("files")]
        public required IReadOnlyCollection<UploadSessionFileRequest> Files { get; init; }

        [JsonPropertyName("repliesCount")]
        public required int RepliesCount { get; init; }
    }
}
