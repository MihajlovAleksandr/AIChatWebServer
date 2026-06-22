using AIChatWebServer.Services.Interfaces.Payments;
using Stripe;

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
                Amount = ConvertToMinorUnits(amount),
                Currency = currency.ToLowerInvariant(),

                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                },

                Metadata = new Dictionary<string, string>
                {
                    ["paymentId"] = paymentId.ToString()
                }
            };

            var service = new PaymentIntentService();

            var paymentIntent = await service.CreateAsync(
                options,
                cancellationToken: ct);

            return paymentIntent.ClientSecret;
        }

        public async Task<MobileSubscriptionResponse> CreateSubscriptionAsync(
            Guid userId,
            Guid paymentId,
            string email,
            string priceId,
            CancellationToken ct)
        {
            var customer = await CreateCustomerAsync(
                userId,
                email,
                ct);

            var ephemeralKey = await CreateEphemeralKeyAsync(
                customer.Id,
                ct);

            var subscription = await CreateSubscriptionAsyncInternal(
                customer.Id,
                userId,
                paymentId,
                priceId,
                ct);

            var clientSecret =
                subscription.LatestInvoice?
                    .ConfirmationSecret?
                    .ClientSecret
                ?? throw new InvalidOperationException(
                    "Stripe subscription confirmation secret not found.");

            return new MobileSubscriptionResponse
            {
                CustomerId = customer.Id,
                EphemeralKey = ephemeralKey.Secret,
                SubscriptionId = subscription.Id,
                ClientSecret = clientSecret,
                PublishableKey = _config["Stripe:PublishableKey"]!
            };
        }

        public async Task CancelSubscriptionAsync(
            string subscriptionId,
            CancellationToken ct)
        {
            var service = new SubscriptionService();

            await service.CancelAsync(
                subscriptionId,
                cancellationToken: ct);
        }

        private async Task<Customer> CreateCustomerAsync(
            Guid userId,
            string email,
            CancellationToken ct)
        {
            var service = new CustomerService();

            var options = new CustomerCreateOptions
            {
                Email = email,

                Metadata = new Dictionary<string, string>
                {
                    ["userId"] = userId.ToString()
                }
            };

            return await service.CreateAsync(
                options,
                cancellationToken: ct);
        }

        private async Task<EphemeralKey> CreateEphemeralKeyAsync(
            string customerId,
            CancellationToken ct)
        {
            var service = new EphemeralKeyService();

            var options = new EphemeralKeyCreateOptions
            {
                Customer = customerId
            };

            return await service.CreateAsync(
                options,
                cancellationToken: ct);
        }

        private async Task<Subscription> CreateSubscriptionAsyncInternal(
            string customerId,
            Guid userId,
            Guid paymentId,
            string priceId,
            CancellationToken ct)
        {
            var service = new SubscriptionService();

            var options = new SubscriptionCreateOptions
            {
                Customer = customerId,

                Items = new List<SubscriptionItemOptions>
                {
                    new()
                    {
                        Price = priceId
                    }
                },

                PaymentBehavior = "default_incomplete",

                PaymentSettings = new SubscriptionPaymentSettingsOptions
                {
                    SaveDefaultPaymentMethod = "on_subscription"
                },

                Expand = new List<string>
                {
                    "latest_invoice.confirmation_secret"
                },

                Metadata = new Dictionary<string, string>
                {
                    ["userId"] = userId.ToString(),
                    ["paymentId"] = paymentId.ToString()
                }
            };

            return await service.CreateAsync(
                options,
                cancellationToken: ct);
        }

        private static long ConvertToMinorUnits(decimal amount)
        {
            return (long)(amount * 100m);
        }
    }

    public sealed class MobileSubscriptionResponse
    {
        public string CustomerId { get; init; } = null!;

        public string EphemeralKey { get; init; } = null!;

        public string SubscriptionId { get; init; } = null!;

        public string ClientSecret { get; init; } = null!;

        public string PublishableKey { get; init; } = null!;
    }
}