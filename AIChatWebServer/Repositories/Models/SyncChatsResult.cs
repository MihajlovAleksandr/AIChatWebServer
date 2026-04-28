using AIChatWebServer.Models.Chats;

namespace AIChatWebServer.Repositories.Models
{
    public record SyncChatsResult(
        IReadOnlyList<Chat> Created,
        IReadOnlyList<Chat> Updated,
        IReadOnlyList<Guid> Deleted
    );
}