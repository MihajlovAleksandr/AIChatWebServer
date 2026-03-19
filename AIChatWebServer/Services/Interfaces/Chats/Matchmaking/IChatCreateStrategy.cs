namespace AIChatWebServer.Services.Interfaces.Chats.Matchmaking
{
    public interface IChatCreateStrategy
    {
        Task<Guid> CreateAsync(Guid userId, string chatName, CancellationToken ct);
    }
}
