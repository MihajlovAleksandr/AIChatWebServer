using AIChatWebServer.Models.Payment;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.Payments;
using Npgsql;

namespace AIChatWebServer.Services.Implementations.Payments
{
    public class SingleItemProductRule(IPaymentItemRepository paymentItemRepository) : IProductAccessRule
    {
        private readonly IPaymentItemRepository _paymentItemRepository = paymentItemRepository;

        public bool CanHandle(PaymentType type)
        {
            return type == PaymentType.SingleItem;
        }

        public async Task<bool> Handle(Product product, Guid userId, CancellationToken ct = default)
        {
            return !await _paymentItemRepository.HasUserPurchasedProductAsync(userId, product.Id, ct);
        }

        public IProductAccessRule WithTransaction(NpgsqlConnection conn, NpgsqlTransaction tx)
        {
            return new SingleItemProductRule(_paymentItemRepository.WithTransaction(conn, tx));
        }
    }
}
