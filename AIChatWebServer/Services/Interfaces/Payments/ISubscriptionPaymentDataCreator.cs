using AIChatWebServer.Models.Payment;

namespace AIChatWebServer.Services.Interfaces.Payments
{
    public interface ISubscriptionPaymentDataCreator
    {
        Task<SubscriptionPaymentData> Create(string subscriptionId, CancellationToken ct = default);
    }
}
