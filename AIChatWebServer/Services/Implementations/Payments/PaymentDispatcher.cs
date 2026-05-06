using AIChatWebServer.Models.Exceptions.Implementations.Payment;
using AIChatWebServer.Models.Payment;
using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Interfaces;
using AIChatWebServer.Services.Interfaces.Payments;

namespace AIChatWebServer.Services.Implementations.Payments
{
    public class PaymentDispatcher(
        IPurchaseService purchaseService,
        IUserPremiumService userPremiumService,
        IPaymentOrchestrator stripeOrchestrator)
        : IPaymentDispatcher
    {
        private readonly IPurchaseService _purchaseService = purchaseService;
        private readonly IUserPremiumService _userPremiumService = userPremiumService;
        private readonly IPaymentOrchestrator _stripe = stripeOrchestrator;

        public async Task<PaymentCredentials> CreatePayment(
            List<(Guid productId, int quantity)> paymentItems,
            PaymentType paymentType,
            Guid userId,
            CancellationToken ct)
        {
            if (paymentType == PaymentType.Subscription)
            {
                if (paymentItems.Count != 1)
                    throw new InvalidSubscriptionPaymentItemsException(paymentItems.Count);

                UserPremium? userPremium = await _userPremiumService.GetAutoRenewAsync(userId, ct);
                if (userPremium != null)
                {
                    throw new UserAlreadyHasAutoRenewSubscriptionException(userId);
                }
            }

            var (paymentId, amount, currency, items) = await _purchaseService.CreatePaymentAsync(
                userId,
                paymentItems,
                ct);

            var firstItem = items.First();

            if (firstItem.Product.Type == PaymentType.Subscription)
            {
                var url = await _stripe.CreateSubscriptionAsync(
                    userId,
                    paymentId,
                    firstItem.Product.StripePriceId,
                    ct);

                return new PaymentCredentials(PaymentType.Subscription, new { url });
            }

            var clientSecret = await _stripe.CreateOneTimePaymentAsync(
                paymentId,
                amount,
                currency,
                ct);

            return new PaymentCredentials(PaymentType.Payment,
                 new
                 {
                     clientSecret,
                     paymentId
                 }
            );
        }

        public async Task CancelSubscription(
            Guid userId,
            CancellationToken ct)
        {
            UserPremium? premium = await _userPremiumService.GetAutoRenewAsync(userId, ct)
                ?? throw new UserAutoRenewSubscriptionNotFoundException(userId);

            if (premium.SubscriptionId == null)
                throw new ArgumentException();

            await _stripe.CancelSubscriptionAsync(premium.SubscriptionId, ct);
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
            return _purchaseService
                .GetPaymentDetailsAsync(id, ct);
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
    }
}
