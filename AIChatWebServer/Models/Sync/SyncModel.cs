namespace AIChatWebServer.Models.Sync
{
    public sealed record SyncModel
    (
        SyncMatchmaking SyncMatchmaking,
        SyncMessages SyncMessages,
        SyncChats SyncChats
    );
}
