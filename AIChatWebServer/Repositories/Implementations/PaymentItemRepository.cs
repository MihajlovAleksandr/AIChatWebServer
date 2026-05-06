using AIChatWebServer.Models.Payment;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class PaymentItemRepository : BaseRepository, IPaymentItemRepository
    {
        private readonly NpgsqlConnection? _conn;
        private readonly NpgsqlTransaction? _tx;

        public PaymentItemRepository() { }

        private PaymentItemRepository(NpgsqlConnection conn, NpgsqlTransaction tx)
        {
            _conn = conn;
            _tx = tx;
        }

        public IPaymentItemRepository WithTransaction(NpgsqlConnection conn, NpgsqlTransaction tx)
            => new PaymentItemRepository(conn, tx);

        public Task CreateAsync(PaymentItem item, CancellationToken ct = default)
        {
            return ExecuteAsync(
                PaymentItemQueries.Insert,
                ct,
                ("@id", item.Id),
                ("@payment_id", item.PaymentId),
                ("@product_id", item.Product.Id),
                ("@quantity", item.Quantity),
                ("@price", item.Price)
            );
        }

        public Task<List<PaymentItem>> GetByPaymentAsync(Guid paymentId, string region, CancellationToken ct = default)
        {
            return ReadAsync(
                PaymentItemQueries.GetByPayment,
                region,
                ct,
                ("@payment_id", paymentId),
                ("@region", region));
        }

        private async Task<List<PaymentItem>> ReadAsync(
            string sql,
            string region,
            CancellationToken ct,
            params (string, object)[] parameters)
        {
            var list = new List<PaymentItem>();

            var conn = _conn ?? await GetConnectionAsync(ct);

            await using var cmd = new NpgsqlCommand(sql, conn, _tx);

            foreach (var (n, v) in parameters)
                cmd.Parameters.AddWithValue(n, v ?? DBNull.Value);

            await using var reader = await cmd.ExecuteReaderAsync(ct);

            while (await reader.ReadAsync(ct))
                list.Add(Map(reader));

            return list;
        }

        private static PaymentItem Map(NpgsqlDataReader r)
        {
            var product = new Product
            {
                Id = r.GetGuid(r.GetOrdinal("p_id")),
                Code = r.GetString(r.GetOrdinal("code")),
                Name = r.GetString(r.GetOrdinal("name")),
                Description = r.GetString(r.GetOrdinal("description")),
                Type = Enum.Parse<PaymentType>(r.GetString(r.GetOrdinal("type"))),
                Price = r.GetDecimal(r.GetOrdinal("product_price")),
                Currency = r.GetString(r.GetOrdinal("currency")),
                StripePriceId = r.GetString(r.GetOrdinal("stripe_price_id")),

                AttributesJson = r.GetString(r.GetOrdinal("attributes")),
                IsActive = r.GetBoolean(r.GetOrdinal("is_active")),
                CreatedAt = r.GetDateTime(r.GetOrdinal("created_at"))
            };

            return new PaymentItem
            {
                Id = r.GetGuid(r.GetOrdinal("id")),
                PaymentId = r.GetGuid(r.GetOrdinal("payment_id")),
                Product = product,
                Quantity = r.GetInt32(r.GetOrdinal("quantity")),
                Price = r.GetDecimal(r.GetOrdinal("price"))
            };
        }

        private async Task ExecuteAsync(string sql, CancellationToken ct, params (string, object)[] parameters)
        {
            var conn = _conn ?? await GetConnectionAsync(ct);

            await using var cmd = new NpgsqlCommand(sql, conn, _tx);

            foreach (var (n, v) in parameters)
                cmd.Parameters.AddWithValue(n, v ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync(ct);
        }
    }
}