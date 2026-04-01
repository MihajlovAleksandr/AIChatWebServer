using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record MessageReplyResponse
    (
        [property: JsonPropertyName("replyMessageId")] Guid ReplyMessageId,
        [property: JsonPropertyName("startIndexQuote")] int? StartIndexQuote,
        [property: JsonPropertyName("endIndexQuote")] int? EndIndexQuote
    );
}
