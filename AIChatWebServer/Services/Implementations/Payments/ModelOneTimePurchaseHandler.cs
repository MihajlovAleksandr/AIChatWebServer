using AIChatWebServer.Models.Payment;
using AIChatWebServer.Services.Interfaces.AI;
using AIChatWebServer.Services.Interfaces.Payments;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Npgsql;

namespace AIChatWebServer.Services.Implementations.Payments
{
    public class ModelOneTimePurchaseHandler : IPurchaseHandler
    {
        private readonly ModelPaymentSettings _settings;
        private readonly IUserAiService _userAiService;
        private readonly NpgsqlConnection? _conn;
        private readonly NpgsqlTransaction? _tx;

        public ModelOneTimePurchaseHandler(IOptions<ModelPaymentSettings> settings, IUserAiService userAiService)
        {
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _userAiService = userAiService;
        }

        private ModelOneTimePurchaseHandler(
            IOptions<ModelPaymentSettings> settings,
            IUserAiService userAiService,
            NpgsqlConnection conn,
            NpgsqlTransaction tx) : this(settings, userAiService)
        {
            _conn = conn;
            _tx = tx;
        }

        public IPurchaseHandler WithTransaction(NpgsqlConnection conn, NpgsqlTransaction tx)
        {
            return new ModelOneTimePurchaseHandler(
                Microsoft.Extensions.Options.Options.Create(_settings),
                _userAiService.WithTransaction(conn, tx),
                conn,
                tx);
        }

        public async Task ApplyAsync(
            Guid userId,
            PaymentItem item,
            PaymentData data,
            CancellationToken ct)
        {
            if (_tx == null)
                throw new InvalidOperationException("ApplyAsync must be executed within a transaction");

            var attrs = JsonSerializer.Deserialize<ModelAtributes>(item.Product.AttributesJson)
                ?? throw new ArgumentException("Invalid json attributes");

            await _userAiService.CreateAsync(userId, attrs.Model, item.Id, ct);
        }

        public bool CanHandle(Product product, PaymentData data)
        {
            return product.Type == PaymentType.SingleItem
                && data is OneTimePaymentData
                && _settings.SupportedCodes.Contains(product.Code);
        }
    }
}