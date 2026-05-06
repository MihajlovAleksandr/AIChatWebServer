namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        T WithTransaction<T>(ITransactionRepository<T> repository) where T: ITransactionRepository<T>;

        Task CommitAsync(
            CancellationToken ct = default);

        Task RollbackAsync(
            CancellationToken ct = default);
    }
}