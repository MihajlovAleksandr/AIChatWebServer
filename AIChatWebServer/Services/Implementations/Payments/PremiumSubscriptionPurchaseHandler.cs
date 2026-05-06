using AIChatWebServer.Models.Payment;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.Payments;
using System.Text.Json;

namespace AIChatWebServer.Services.Implementations.Payments
{
    public sealed class PremiumSubscriptionPurchaseHandler(IUserPremiumRepository userPremiumRepository) : IPurchaseHandler
    {
        private readonly IUserPremiumRepository _userPremiumRepository = userPremiumRepository;

        public bool CanHandle(Product product, PaymentData data)
            => product.Type == PaymentType.Subscription && data is SubscriptionPaymentData;

        public async Task ApplyAsync(
            Guid userId,
            PaymentItem item,
            PaymentData data,
            IUnitOfWork uow,
            CancellationToken ct)
        {
            var subData = (SubscriptionPaymentData)data;

            var premiumRepo = uow.WithTransaction(_userPremiumRepository);

            var last = await premiumRepo.GetLastAsync(userId, ct);
            var now = DateTime.UtcNow;

            var start = last == null || last.EndTime < now
                ? now
                : last.EndTime;

            var attrs = JsonSerializer.Deserialize<SubsAtributes>(item.Product.AttributesJson)
                ?? throw new ArgumentException("Invalid json attributes");

            var end = start.AddDays(attrs.Days);

            await premiumRepo.CreateAsync(
                userId,
                item.Id,
                start,
                end,
                isAutoRenew: subData.IsAutoRenew,
                subscriptionId: subData.SubscriptionId,
                ct: ct);
        }
    }
}