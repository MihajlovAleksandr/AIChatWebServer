using AIChatWebServer.Contracts.UnitOfWork.Interfaces;
using AIChatWebServer.Models.Payment;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IPaymentItemRepository : ITransactionalScope<IPaymentItemRepository>
    {
        Task CreateAsync(PaymentItem item, CancellationToken ct = default);
        Task<List<PaymentItem>> GetByPaymentAsync(Guid paymentId, string region, CancellationToken ct = default);
        Task<bool> HasUserPurchasedProductAsync(Guid userId, Guid productId, CancellationToken ct = default);
    }
}