namespace AIChatWebServer.Contracts.UnitOfWork.Interfaces
{
    public interface IUnitOfWorkFactory
    {
        Task<IUnitOfWork> CreateAsync(
            CancellationToken ct = default);
    }
}