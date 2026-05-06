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
            string priceId,
            decimal amount,
            string currency,
            CancellationToken ct);

        Task ConfirmPaymentAsync(Guid paymentId, string externalTransactionId, PaymentData data, CancellationToken ct);

        Task<List<Payment>> GetUserPaymentsAsync(Guid userId, CancellationToken ct);

        Task<(Payment payment, List<PaymentItem> items)> GetPaymentDetailsAsync(
             Guid paymentId,
             CancellationToken ct);
    }
}