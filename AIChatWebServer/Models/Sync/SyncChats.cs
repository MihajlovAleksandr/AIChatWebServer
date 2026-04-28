using AIChatWebServer.Models.Chats;

namespace AIChatWebServer.Models.Sync
{
    public sealed record SyncChats
    (
        IReadOnlyList<ChatWithUserContext> NewChats,
        IReadOnlyList<ChatWithUserContext> UpdatedChats,
        IReadOnlyList<Guid> DeletedChats
    );
}
