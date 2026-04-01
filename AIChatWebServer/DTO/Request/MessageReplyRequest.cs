using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public sealed record MessageReplyRequest
    {
        [JsonPropertyName("replyMessageId")]
        public required Guid ReplyMessageId { get; init; }
        [JsonPropertyName("startIndexQuote")]
        public int? StartIndexQuote { get; init; } = null;
        [JsonPropertyName("endIndexQuote")]
        public int? EndIndexQuote { get; init; } = null;
    }
}
