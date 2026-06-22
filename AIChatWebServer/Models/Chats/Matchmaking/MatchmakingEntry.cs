namespace AIChatWebServer.Models.Chats.Matchmaking
{
    public sealed record MatchmakingEntry(
        Guid Id,
        Guid UserId,
        ChatType ChatType,
        string ChatName,
        string MatchPredicate,
        ChatMatchStatus Status,
        DateTime CreatedAt,
        DateTime? ExpiresAt
    );
}