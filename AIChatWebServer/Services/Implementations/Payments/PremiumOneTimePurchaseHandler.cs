using AIChatWebServer.Contracts.UnitOfWork.Interfaces;
using AIChatWebServer.Models.Payment;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.Payments;
using System.Text.Json;
using Npgsql;

namespace AIChatWebServer.Services.Implementations.Payments
{
    public sealed class PremiumOneTimePurchaseHandler : IPurchaseHandler
    {
        private readonly IUserPremiumRepository _userPremiumRepository;
        private readonly NpgsqlConnection? _conn;
        private readonly NpgsqlTransaction? _tx;

        public PremiumOneTimePurchaseHandler(IUserPremiumRepository userPremiumRepository)
        {
            _userPremiumRepository = userPremiumRepository;
        }

        private PremiumOneTimePurchaseHandler(
            IUserPremiumRepository userPremiumRepository,
            NpgsqlConnection conn,
            NpgsqlTransaction tx) : this(userPremiumRepository)
        {
            _conn = conn;
            _tx = tx;
        }

        public IPurchaseHandler WithTransaction(NpgsqlConnection conn, NpgsqlTransaction tx)
        {
            return new PremiumOneTimePurchaseHandler(
                _userPremiumRepository.WithTransaction(conn, tx),
                conn,
                tx);
        }

        public bool CanHandle(Product product, PaymentData data)
            => product.Type == PaymentType.Subscription && data is OneTimePaymentData;

        public async Task ApplyAsync(
            Guid userId,
            PaymentItem item,
            PaymentData data,
            CancellationToken ct)
        {
            if (_tx == null)
                throw new InvalidOperationException("ApplyAsync must be executed within a transaction");

            var subData = (OneTimePaymentData)data;

            var last = await _userPremiumRepository.GetLastAsync(userId, ct);
            var now = DateTime.UtcNow;

            var start = last == null || last.EndTime < now
                ? now
                : last.EndTime;

            var attrs = JsonSerializer.Deserialize<SubsAtributes>(item.Product.AttributesJson)
                ?? throw new ArgumentException("Invalid json attributes");

            DateTime end = attrs.Apply(start);

            await _userPremiumRepository.CreateAsync(
                userId,
                item.Id,
                start,
                end,
                ct: ct);
        }
    }
}