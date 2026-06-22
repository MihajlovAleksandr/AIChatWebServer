namespace AIChatWebServer.Models.Messages
{
    public sealed class MessageReply(
        Guid replyMessageId,
        int? startIndexQuote,
        int? endIndexQuote)
    {
        public Guid ReplyMessageId { get; init; } = replyMessageId;

        public int? StartIndexQuote { get; init; } = startIndexQuote;

        public int? EndIndexQuote { get; init; } = endIndexQuote;
    }
}