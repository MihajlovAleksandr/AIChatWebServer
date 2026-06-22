using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public record MessageRequest
    {
        [JsonPropertyName("id")]
        public required Guid Id { get; init; }
        [JsonPropertyName("chatId")]
        public required Guid ChatId { get; init; }
        [JsonPropertyName("text")]
        public required string Text { get; init; }
        [JsonPropertyName("replies")]
        public IReadOnlyCollection<MessageReplyRequest> Replies { get; init; } = new List<MessageReplyRequest>();

        [JsonPropertyName("uploadSessionId")]
        public Guid? UploadSessionId { get; init; } = null;
    }   
}
