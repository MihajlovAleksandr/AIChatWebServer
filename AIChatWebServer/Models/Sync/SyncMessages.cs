using AIChatWebServer.Models.Messages;

namespace AIChatWebServer.Models.Sync
{
    public record SyncMessages
    (
        IReadOnlyList<MessageContext> NewMessages,
        IReadOnlyList<MessageContext> UpdatedMessages,
        IReadOnlyList<Guid> DeletedMessages
    );
}
