using AIChatWebServer.Models.Payment;
using AIChatWebServer.Services.Interfaces.Payments;
using Stripe;

namespace AIChatWebServer.Services.Implementations.Payments;

public class SubscriptionPaymentDataCreator(
    SubscriptionService subscriptionService)
    : ISubscriptionPaymentDataCreator
{
    private readonly SubscriptionService _subscriptionService =
        subscriptionService;

    public async Task<SubscriptionPaymentData> Create(
        string subscriptionId,
        CancellationToken ct = default)
    {
        var subscription =
            await _subscriptionService.GetAsync(
                subscriptionId,
                cancellationToken: ct);

        var currentItem =
            GetCurrentItem(subscription);

        return new SubscriptionPaymentData(
            subscriptionId,
            WillAutoCharge(subscription),
            subscription.Created,
            currentItem.CurrentPeriodStart,
            currentItem.CurrentPeriodEnd);
    }

    private static bool WillAutoCharge(
        Subscription subscription)
    {
        return
            subscription.CollectionMethod ==
            "charge_automatically"
            && subscription.Status is
                "active"
                or "trialing"
            && !subscription.CancelAtPeriodEnd;
    }

    private static SubscriptionItem GetCurrentItem(
        Subscription subscription)
    {
        return subscription.Items.Data.FirstOrDefault()
            ?? throw new InvalidOperationException(
                $"Subscription '{subscription.Id}' does not contain subscription items.");
    }
}