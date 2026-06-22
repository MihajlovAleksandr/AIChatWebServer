using AIChatWebServer.Models.Messages;

namespace AIChatWebServer.Repositories.Models
{
    public record SyncMessagesResult(
        IReadOnlyList<Message> Created,
        IReadOnlyList<Message> Updated,
        IReadOnlyList<Guid> Deleted
    );
}