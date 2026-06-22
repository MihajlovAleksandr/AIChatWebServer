using AIChatWebServer.Models.Messages;
using AIChatWebServer.Repositories.Models;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IMessageRepository
    {
        Task<Guid> CreateAsync(
            Guid id,
            Guid chatId,
            Guid userId,
            string text,
            IReadOnlyCollection<Guid> chatUserIds,
            IReadOnlyCollection<Guid> fileIds,
            IReadOnlyCollection<MessageReply> replies,
            CancellationToken ct = default);

        Task<Message?> GetById(
            Guid id,
            CancellationToken ct = default);

        Task<Message?> GetByIdWithDependenciesAsync(
            Guid id,
            CancellationToken ct = default);

        Task<IReadOnlyList<Message>> GetByChatWithDependenciesAsync(
            Guid chatId,
            CancellationToken ct = default);

        Task UpdateTextAsync(
            Guid id,
            string text,
            DateTime updatedAt,
            CancellationToken ct = default);

        Task UpdateStatusAsync(
            Guid messageId,
            Guid userId,
            MessageStatus status,
            DateTime updatedAt,
            CancellationToken ct = default);

        Task DeleteFileAsync(
            Guid messageId,
            Guid fileId,
            CancellationToken ct = default);

        Task DeleteMessageAsync(
            Guid id,
            CancellationToken ct = default);

        Task<SyncMessagesResult> GetChangesAsync(
                IReadOnlyCollection<Guid> chatIds,
                DateTime since,
                CancellationToken ct);

    }
}