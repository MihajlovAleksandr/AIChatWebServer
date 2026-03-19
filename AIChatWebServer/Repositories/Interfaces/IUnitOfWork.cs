namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IMatchmakingRepository Matchmaking { get; }

        IGroupChatSearchRepository GroupChatSearch { get; }

        Task CommitAsync(
            CancellationToken ct = default);

        Task RollbackAsync(
            CancellationToken ct = default);
    }
}