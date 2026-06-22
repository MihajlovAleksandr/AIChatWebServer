namespace AIChatWebServer.Models.Sync
{
    public sealed record SyncGroupMatchmaking
    (
        bool IsSearching,
        Guid? ChatId
    );
}
