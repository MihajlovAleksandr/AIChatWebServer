using AIChatWebServer.Models.Payment;
using AIChatWebServer.Models.User;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.Payments;
using System.Text.Json;
using Npgsql;

namespace AIChatWebServer.Services.Implementations.Payments
{
    public sealed class PremiumSubscriptionPurchaseHandler : IPurchaseHandler
    {
        private readonly IUserPremiumRepository _userPremiumRepository;
        private readonly NpgsqlConnection? _conn;
        private readonly NpgsqlTransaction? _tx;

        public PremiumSubscriptionPurchaseHandler(IUserPremiumRepository userPremiumRepository)
        {
            _userPremiumRepository = userPremiumRepository;
        }

        private PremiumSubscriptionPurchaseHandler(
            IUserPremiumRepository userPremiumRepository,
            NpgsqlConnection conn,
            NpgsqlTransaction tx) : this(userPremiumRepository)
        {
            _conn = conn;
            _tx = tx;
        }

        public IPurchaseHandler WithTransaction(NpgsqlConnection conn, NpgsqlTransaction tx)
        {
            return new PremiumSubscriptionPurchaseHandler(
                _userPremiumRepository.WithTransaction(conn, tx),
                conn,
                tx);
        }

        public bool CanHandle(Product product, PaymentData data)
            => product.Type == PaymentType.Subscription && data is SubscriptionPaymentData;

        public async Task ApplyAsync(
            Guid userId,
            PaymentItem item,
            PaymentData data,
            CancellationToken ct)
        {
            if (_tx == null)
                throw new InvalidOperationException("ApplyAsync must be executed within a transaction");

            var subData = (SubscriptionPaymentData)data;

            var attrs = JsonSerializer.Deserialize<SubsAtributes>(item.Product.AttributesJson)
                ?? throw new ArgumentException("Invalid json attributes");

            if (subData.CurrentPeriodEnd != attrs.Apply(subData.CurrentPeriodStart))
                throw new ArgumentException("Period end doesn't match expected value");

            if (subData.CurrentPeriodStart != subData.CreatedAt)
            {
                UserPremium premium = await _userPremiumRepository.GetFirstBySubscriptionIdAsync(subData.SubscriptionId, ct)
                    ?? throw new ArgumentException("Premium subscription not found");

                if (premium.StartTime != subData.CreatedAt)
                    throw new ArgumentException("Start time mismatch");
            }

            await _userPremiumRepository.CreateAsync(
                userId,
                item.Id,
                subData.CurrentPeriodStart,
                subData.CurrentPeriodEnd,
                isAutoRenew: subData.WillAutoCharge,
                subscriptionId: subData.SubscriptionId,
                ct: ct);
        }
    }
}