namespace AIChatWebServer.Models.Chats.Matchmaking
{
    public sealed record GroupChatSearchEntry(
        Guid Id,
        Guid ChatId,
        Guid UserId,
        string MatchPredicate,
        int Slots,
        ChatMatchStatus Status,
        DateTime CreatedAt
    );
}