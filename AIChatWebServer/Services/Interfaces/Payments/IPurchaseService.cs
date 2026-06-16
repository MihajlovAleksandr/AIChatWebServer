using AIChatWebServer.Contracts.UnitOfWork.Interfaces;
using AIChatWebServer.Models.Payment;

namespace AIChatWebServer.Services.Interfaces.Payments
{
    public interface IPurchaseService
    {
        Task<List<Product>> GetAvailableProductsAsync(
            Guid userId,
            string? type,
            CancellationToken ct);

        Task<Product> GetProductAsync(
            Guid productId,
            Guid userId,
            CancellationToken ct);

        Task<(Guid paymentId, decimal amount, string currency, List<PaymentItem> items)> CreatePaymentAsync(
                    Guid userId,
                    List<(Guid productId, int quantity)> items,
                    CancellationToken ct);

        Task ProcessSubscriptionRenewalAsync(
                    string subscriptionId,
                    string invoiceId,
                    string stripeInvoiceUrl,
                    string priceId,
                    decimal amount,
                    string currency,
                    PaymentData paymentData,
                    CancellationToken ct);

        Task ConfirmPaymentAsync(
            Guid paymentId,
            string invoiceId,
            string? stripeChargeId,
            string? stripeInvoiceUrl,
            PaymentData data,
            CancellationToken ct);

        Task<List<Payment>> GetUserPaymentsAsync(Guid userId, CancellationToken ct);

        Task<(Payment payment, List<PaymentItem> items)> GetPaymentDetailsAsync(
             Guid paymentId,
             CancellationToken ct);

        Task UpdateReceiptUrlAsync(
            string stripeChargeId,
            string stripeInvoiceUrl,
            CancellationToken ct);

        Task<(Payment payment, List<PaymentItem> items)> GetPremiumPaymentDetailsAsync(
            Guid premiumId,
            CancellationToken ct);

        Task FailPaymentAsync(
            Guid paymentId,
            string? invoiceId,
            string? stripeChargeId,
            string? stripeInvoiceUrl,
            CancellationToken ct);

        Task FailSubscriptionRenewalAsync(
            string subscriptionId,
            string invoiceId,
            string? stripeChargeId,
            string? stripeInvoiceUrl,
            CancellationToken ct);
    }
}