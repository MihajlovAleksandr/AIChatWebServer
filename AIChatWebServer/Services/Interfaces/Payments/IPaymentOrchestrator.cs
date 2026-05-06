namespace AIChatWebServer.Services.Interfaces.Payments
{
    public interface IPaymentOrchestrator
    {
        Task<string> CreateOneTimePaymentAsync(Guid paymentId, decimal amount, string currency, CancellationToken ct);

        Task<string> CreateSubscriptionAsync(
            Guid userId,
            Guid paymentId,
            string priceId,
            CancellationToken ct);

        Task CancelSubscriptionAsync(string subscriptionId, CancellationToken ct);
    }
}
