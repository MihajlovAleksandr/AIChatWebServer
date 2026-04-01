using AIChatWebServer.Models.Files;
using AIChatWebServer.Models.Messages;

namespace AIChatWebServer.Services.Interfaces
{
    public interface IMessageService
    {
        Task<UploadSession> PrepareAsync(
            Guid chatId, Guid userId, int textLength,
            IReadOnlyCollection<UploadSessionFile> files,
            int repliesCount, CancellationToken ct);
        Task<MessageContext> CreateAsync(
            Guid messageId,
            Guid chatId, Guid userId, string text,
            Guid? uploadSessionId,
            IReadOnlyCollection<MessageReply> replies,
            CancellationToken ct);
        Task<MessageContext> GetById(
            Guid messageId, Guid userId, 
            CancellationToken ct);
        Task<IEnumerable<MessageContext>> GetByChatId(
            Guid chatId, Guid userId,
            CancellationToken ct);
        Task EditText(
            Guid messageId, string text, 
            Guid userId, CancellationToken ct);

        Task DeleteFile(
            Guid messageId,
            Guid fileId,
            Guid userId,
            CancellationToken ct);

        Task DeleteMessage(
            Guid messageId, Guid userId, 
            CancellationToken ct);
        Task EditMessagesStatus(
            IReadOnlyCollection<Guid> messageIds, 
            MessageStatus status, Guid userId, 
            CancellationToken ct);
    }
}
