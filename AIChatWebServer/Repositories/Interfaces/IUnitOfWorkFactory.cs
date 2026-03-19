namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IUnitOfWorkFactory
    {
        Task<IUnitOfWork> CreateAsync(
            CancellationToken ct = default);
    }
}