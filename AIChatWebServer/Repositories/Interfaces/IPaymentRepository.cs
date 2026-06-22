using AIChatWebServer.Contracts.UnitOfWork.Interfaces;
using AIChatWebServer.Models.Payment;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IPaymentRepository : ITransactionalScope<IPaymentRepository>
    {
        Task CreateAsync(
            Payment payment,
            CancellationToken ct = default);

        Task ConfirmAsync(
            Guid paymentId,
            string transactionId,
            string? stripeChargeId,
            string? stripeInvoiceUrl,
            CancellationToken ct = default);

        Task FailAsync(
            Guid paymentId,
            string? transactionId = null,
            string? stripeChargeId = null,
            string? stripeInvoiceUrl = null,
            CancellationToken ct = default);

        Task UpdateReceiptUrlAsync(
            string stripeChargeId,
            string stripeInvoiceUrl,
            CancellationToken ct = default);

        Task<Payment?> GetByIdAsync(
            Guid id,
            CancellationToken ct = default);

        Task<Payment?> GetByStripeChargeIdAsync(
            string stripeChargeId,
            CancellationToken ct = default);

        Task<List<Payment>> GetHistoryAsync(
            Guid userId,
            CancellationToken ct = default);

        Task<bool> ExistsByTransactionId(
            string transactionId,
            CancellationToken ct = default);

        Task<Payment?> GetByPremiumIdAsync(
            Guid premiumId,
            CancellationToken ct = default);

        Task ExpirePendingPaymentsAsync(
            DateTime expiredBeforeUtc,
            CancellationToken ct = default);
    }
}