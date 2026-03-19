namespace AIChatWebServer.Services.Interfaces.Chats.Matchmaking
{
    public interface IChatAddUserStrategy : IChatMatchStrategy
    {
        Task MatchChatAsync(
            Guid userId, 
            Guid chatId, 
            int slot, 
            string userPredicate,
            CancellationToken ct);
    }
}
