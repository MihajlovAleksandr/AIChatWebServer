using AIChatWebServer.Models.Payment;

namespace AIChatWebServer.Services.Interfaces.Payments
{
    public interface IPaymentDispatcher
    {
        Task<PaymentCredentials> CreatePayment(
            List<(Guid productId, int quantity)> paymentItems,
            PaymentType paymentType,
            Guid userId,
            CancellationToken ct);

        Task CancelSubscription(
            Guid userId,
            CancellationToken ct);

        Task<List<Payment>> GetMyPayments(
            Guid userId,
            CancellationToken ct);

        Task<(Payment, List<PaymentItem>)> GetPayment(
            Guid id,
            CancellationToken ct);

        Task<List<Product>> GetProducts(
            string? type,
            Guid userId,
            CancellationToken ct);
    }
}