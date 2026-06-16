using AIChatWebServer.Contracts.UnitOfWork.Interfaces;
using AIChatWebServer.Models.Payment;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IProductRepository : ITransactionalScope<IProductRepository>
    {
        Task<Product?> GetByIdAsync(
                    Guid id,
                    string region,
                    CancellationToken ct = default);

        Task<Product?> GetByCodeAsync(
            string code,
            string region,
            CancellationToken ct = default);

        Task<List<Product>> GetActiveAsync(
            string region,
            CancellationToken ct = default);

        Task<List<Product>> GetByTypeAsync(
            string type,
            string region,
            bool onlyActive = true,
            CancellationToken ct = default);

        Task<Product?> GetByStripePriceIdAsync(
            string stripePriceId,
            string region,
            CancellationToken ct = default);
    }
}