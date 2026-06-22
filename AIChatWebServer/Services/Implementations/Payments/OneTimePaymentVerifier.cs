using AIChatWebServer.Models.Payment;
using AIChatWebServer.Services.Interfaces.Payments;

namespace AIChatWebServer.Services.Implementations.Payments
{
    public class OneTimePaymentVerifier : IPaymentVerifier
    {
        public bool CanHandle(PaymentType type)
        {
            return type == PaymentType.Payment;
        }

        public Task Verify(List<(Product product, int quantity)> paymentItems, PaymentType paymentType, Guid userId, CancellationToken ct)
        {
            return Task.CompletedTask;
        }
    }
}
