namespace AIChatWebServer.Models.Sync
{
    public sealed record SyncMatchmaking
    (
        SyncChatMatchmaking SyncChatMatchmaking,
        SyncGroupMatchmaking SyncGroupMatchmaking
    );
}
