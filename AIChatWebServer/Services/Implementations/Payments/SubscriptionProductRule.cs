using AIChatWebServer.Models.Payment;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.Payments;
using Npgsql;

namespace AIChatWebServer.Services.Implementations.Payments
{
    public class SubscriptionProductRule(IUserPremiumRepository premiumRepository) : IProductAccessRule
    {
        private readonly IUserPremiumRepository _premiumRepository = premiumRepository;

        public bool CanHandle(PaymentType type)
        {
            return type == PaymentType.Subscription;
        }

        public async Task<bool> Handle(Product product, Guid userId, CancellationToken ct = default)
        {
            var premium = await _premiumRepository.GetActiveAsync(userId, ct);
            return premium == null;
        }

        public IProductAccessRule WithTransaction(NpgsqlConnection conn, NpgsqlTransaction tx)
        {
            return new SubscriptionProductRule(_premiumRepository.WithTransaction(conn, tx));
        }
    }
}
