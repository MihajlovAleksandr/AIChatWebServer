namespace AIChatWebServer.Contracts.UnitOfWork.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        T WithTransaction<T>(ITransactionalScope<T> repository) where T: ITransactionalScope<T>;

        Task CommitAsync(
            CancellationToken ct = default);

        Task RollbackAsync(
            CancellationToken ct = default);
    }
}