using AIChatWebServer.Models.Payment;

namespace AIChatWebServer.Services.Interfaces.Payments
{
    public interface IPaymentVerifier
    {
        Task Verify(
            List<(Product product, int quantity)> paymentItems,
            PaymentType paymentType,
            Guid userId,
            CancellationToken ct);
        bool CanHandle(PaymentType type);
    }
}
