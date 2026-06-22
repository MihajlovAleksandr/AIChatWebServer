using AIChatWebServer.Models.Exceptions.Implementations.Payment;
using AIChatWebServer.Models.Payment;
using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Interfaces.Payments;
using AIChatWebServer.Services.Interfaces.Users;

namespace AIChatWebServer.Services.Implementations.Payments;

public sealed class PaymentDispatcher(
    IPurchaseService purchaseService,
    IUserPremiumService userPremiumService,
    IUserService userService,
    IPaymentOrchestrator stripeOrchestrator,
    IPaymentVerifierFactory paymentVerifierFactory)
    : IPaymentDispatcher
{
    private readonly IPurchaseService _purchaseService = purchaseService;
    private readonly IUserPremiumService _userPremiumService = userPremiumService;
    private readonly IUserService _userService = userService;
    private readonly IPaymentOrchestrator _stripe = stripeOrchestrator;
    private readonly IPaymentVerifierFactory _paymentVerifierFactory = paymentVerifierFactory;

    public async Task<PaymentCredentials> CreatePayment(
        List<(Guid productId, int quantity)> paymentItems,
        PaymentType paymentType,
        Guid userId,
        CancellationToken ct)
    {
        var productQuantityList = new List<(Product product, int quantity)>();

        foreach(var (productId, quantity) in paymentItems)
        {
            Product product = await _purchaseService.GetProductAsync(productId, userId, ct);
            productQuantityList.Add((product, quantity));
        }

        IPaymentVerifier verifier = _paymentVerifierFactory.Create(paymentType);

        await verifier.Verify(productQuantityList, paymentType, userId, ct);

        var (paymentId, amount, currency, items) =
            await _purchaseService.CreatePaymentAsync(
                userId,
                paymentItems,
                ct);

        var firstItem = items.First();

        if (firstItem.Product.Type == PaymentType.Subscription)
        {
            var user = await _userService.GetByIdAsync(userId, ct);

            var subscriptionCredentials =
                await _stripe.CreateSubscriptionAsync(
                    userId,
                    paymentId,
                    user.Email,
                    firstItem.Product.StripePriceId,
                    ct);

            return new PaymentCredentials(
                PaymentType.Subscription,
                new
                {
                    customerId = subscriptionCredentials.CustomerId,
                    ephemeralKey = subscriptionCredentials.EphemeralKey,
                    subscriptionId = subscriptionCredentials.SubscriptionId,
                    clientSecret = subscriptionCredentials.ClientSecret,
                    publishableKey = subscriptionCredentials.PublishableKey,
                    paymentId
                });
        }

        var clientSecret = await _stripe.CreateOneTimePaymentAsync(
            paymentId,
            amount,
            currency,
            ct);

        return new PaymentCredentials(
            PaymentType.Payment,
            new
            {
                clientSecret,
                paymentId
            });
    }

    public async Task CancelSubscription(
        Guid userId,
        CancellationToken ct)
    {
        UserPremium? premium =
            await _userPremiumService.GetAutoRenewAsync(userId, ct)
            ?? throw new UserAutoRenewSubscriptionNotFoundException(userId);

        if (premium.SubscriptionId == null)
            throw new ArgumentException(nameof(premium.SubscriptionId));

        await _stripe.CancelSubscriptionAsync(
            premium.SubscriptionId,
            ct);
    }

    public Task<List<Payment>> GetMyPayments(
        Guid userId,
        CancellationToken ct)
    {
        return _purchaseService.GetUserPaymentsAsync(
            userId,
            ct);
    }

    public Task<(Payment, List<PaymentItem>)> GetPayment(
        Guid id,
        CancellationToken ct)
    {
        return _purchaseService.GetPaymentDetailsAsync(
            id,
            ct);
    }

    public Task<List<Product>> GetProducts(
        string? type,
        Guid userId,
        CancellationToken ct)
    {
        return _purchaseService.GetAvailableProductsAsync(
            userId,
            type,
            ct);
    }

    public async Task<Product> GetProduct(
        Guid productId,
        Guid userId,
        CancellationToken ct = default)
    {
        return await _purchaseService.GetProductAsync(
            productId,
            userId,
            ct);
    }

    public Task<(Payment, List<PaymentItem>)> GetPremiumPayment(
        Guid premiumId,
        CancellationToken ct)
    {
        return _purchaseService.GetPremiumPaymentDetailsAsync(
            premiumId,
            ct);
    }
}