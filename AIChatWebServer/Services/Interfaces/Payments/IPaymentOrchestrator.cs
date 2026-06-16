using AIChatWebServer.Services.Implementations.Payments;

namespace AIChatWebServer.Services.Interfaces.Payments
{
    public interface IPaymentOrchestrator
    {
        Task<string> CreateOneTimePaymentAsync(Guid paymentId, decimal amount, string currency, CancellationToken ct);

        Task<MobileSubscriptionResponse> CreateSubscriptionAsync(
            Guid userId,
            Guid paymentId,
            string email,
            string priceId,
            CancellationToken ct);

        Task CancelSubscriptionAsync(string subscriptionId, CancellationToken ct);
    }
}
