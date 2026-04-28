using AIChatWebServer.DTO.Response;

namespace AIChatWebServer.Hubs.Interfaces
{
    public interface IMessageEventDispatcher
    {
        Task MessageSent(
            Guid userId,
            Guid? excludedConnectionId,
            MessageResponse response,
            CancellationToken ct);

        Task MessageEdited(
            Guid chatId,
            Guid excludedConnectionId,
            MessageUpdatedResponse response,
            CancellationToken ct);

        Task MessageDeleted(
            Guid chatId,
            Guid excludedConnectionId,
            MessageDeletedResponse response,
            CancellationToken ct);

        Task MessageFileDeleted(
            Guid chatId,
            Guid excludedConnectionId,
            MessageFileDeletedResponse response,
            CancellationToken ct);

        Task MessageStatusUpdated(
            Guid userId,
            Guid? excludedConnectionId,
            MessageStatusUpdatedResponse response,
            CancellationToken ct);
    }
}