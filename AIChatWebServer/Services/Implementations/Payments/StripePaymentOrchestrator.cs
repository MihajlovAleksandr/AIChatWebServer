using AIChatWebServer.Services.Interfaces.Payments;
using Stripe;
using Stripe.Checkout;

namespace AIChatWebServer.Services.Implementations.Payments
{
    public sealed class StripePaymentOrchestrator : IPaymentOrchestrator
    {
        private readonly IConfiguration _config;

        public StripePaymentOrchestrator(IConfiguration config)
        {
            _config = config;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }

        public async Task<string> CreateOneTimePaymentAsync(
            Guid paymentId,
            decimal amount,
            string currency,
            CancellationToken ct)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100),
                Currency = currency.ToLower(),
                Metadata = new Dictionary<string, string>
            {
                { "paymentId", paymentId.ToString() }
            }
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options, null, ct);

            return intent.ClientSecret;
        }

        public async Task<string> CreateSubscriptionAsync(
            Guid userId,
            Guid paymentId,
            string priceId,
            CancellationToken ct)
        {
            var options = new SessionCreateOptions
            {
                Mode = "subscription",
                PaymentMethodTypes = new List<string> { "card" },

                LineItems = new List<SessionLineItemOptions>
                {
                    new()
                    {
                        Price = priceId,
                        Quantity = 1
                    }
                },

                Metadata = new Dictionary<string, string>
                {
                    { "userId", userId.ToString() },
                    { "paymentId", paymentId.ToString() }
                },

                SubscriptionData = new SessionSubscriptionDataOptions
                {
                    Metadata = new Dictionary<string, string>
                    {
                        { "paymentId", paymentId.ToString() },
                        { "userId", userId.ToString() }
                    }
                },

                SuccessUrl = _config["Stripe:SuccessUrl"],
                CancelUrl = _config["Stripe:CancelUrl"]
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options, null, ct);

            return session.Url;
        }

        public async Task CancelSubscriptionAsync(string subscriptionId, CancellationToken ct)
        {
            var service = new SubscriptionService();
            await service.CancelAsync(subscriptionId, null, null, ct);
        }
    }
}
