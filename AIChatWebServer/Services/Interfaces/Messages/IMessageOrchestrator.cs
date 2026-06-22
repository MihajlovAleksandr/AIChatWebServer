using AIChatWebServer.Models.Messages;

namespace AIChatWebServer.Services.Interfaces.Messages
{
    public interface IMessageOrchestrator
    {
        Task<MessageContext> SendMessageAsync(
            Guid messageId,
            Guid chatId,
            Guid userId,
            Guid connectionId,
            string text,
            Guid? uploadSessionId,
            IReadOnlyCollection<MessageReply> replies,
            CancellationToken ct);

        Task<MessageContext> EditTextAsync(
            Guid messageId,
            Guid userId,
            Guid connectionId,
            string text,
            CancellationToken ct);

        Task EditStatusAsync(
            IReadOnlyCollection<Guid> messageIds,
            Guid chatId,
            Guid userId,
            Guid connectionId,
            MessageStatus status,
            CancellationToken ct);
    }
}