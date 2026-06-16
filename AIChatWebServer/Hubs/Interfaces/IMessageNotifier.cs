using AIChatWebServer.Models.Messages;

namespace AIChatWebServer.Hubs.Interfaces
{
    public interface IMessageNotifier
    {
        Task MessageSent(
            Message message,
            CancellationToken ct);

        Task MessageEdited(
            Message message,
            Guid senderConnectionId,
            CancellationToken ct);

        Task MessageDeleted(
            Guid messageId,
            Guid chatId,
            Guid senderConnectionId,
            CancellationToken ct);

        Task MessageFileDeleted(
            Guid messageId,
            Guid fileId,
            Guid chatId,
            Guid senderConnectionId,
            CancellationToken ct);

        Task MessageStatusUpdated(
            IReadOnlyCollection<Guid> messages,
            Guid senderUserId,
            Guid senderConnectionId,
            Guid chatId,
            MessageStatus status,
            CancellationToken ct);
    }
}