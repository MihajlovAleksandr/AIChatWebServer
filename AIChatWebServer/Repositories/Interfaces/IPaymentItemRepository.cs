using AIChatWebServer.Models.Payment;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IPaymentItemRepository : ITransactionRepository<IPaymentItemRepository>
    {
        Task CreateAsync(PaymentItem item, CancellationToken ct = default);

        Task<List<PaymentItem>> GetByPaymentAsync(Guid paymentId, string region, CancellationToken ct = default);
    }
}