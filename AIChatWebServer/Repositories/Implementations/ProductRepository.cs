using AIChatWebServer.Models.Payment;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class ProductRepository : BaseRepository, IProductRepository
    {
        private readonly NpgsqlConnection? _conn;
        private readonly NpgsqlTransaction? _tx;

        public ProductRepository() { }

        private ProductRepository(NpgsqlConnection conn, NpgsqlTransaction tx)
        {
            _conn = conn;
            _tx = tx;
        }

        public IProductRepository WithTransaction(NpgsqlConnection conn, NpgsqlTransaction tx)
            => new ProductRepository(conn, tx);

        public async Task<Product?> GetByIdAsync(
            Guid id,
            string region,
            CancellationToken ct = default)
        {
            var list = await ReadAsync(
                ProductQueries.GetById,
                ct,
                ("@id", id),
                ("@region", region));

            return list.FirstOrDefault();
        }

        public async Task<Product?> GetByCodeAsync(
            string code,
            string region,
            CancellationToken ct = default)
        {
            var list = await ReadAsync(
                ProductQueries.GetByCode,
                ct,
                ("@code", code),
                ("@region", region));

            return list.FirstOrDefault();
        }

        public async Task<Product?> GetByStripePriceIdAsync(
            string stripePriceId,
            string region,
            CancellationToken ct = default)
        {
            var list = await ReadAsync(
                ProductQueries.GetByStripePriceId,
                ct,
                ("@stripe_price_id", stripePriceId),
                ("@region", region));

            return list.FirstOrDefault();
        }

        public Task<List<Product>> GetActiveAsync(
            string region,
            CancellationToken ct = default)
        {
            return ReadAsync(
                ProductQueries.GetActive,
                ct,
                ("@region", region));
        }

        public Task<List<Product>> GetByTypeAsync(
            string type,
            string region,
            bool onlyActive = true,
            CancellationToken ct = default)
        {
            return ReadAsync(
                ProductQueries.GetByType,
                ct,
                ("@type", type),
                ("@only_active", onlyActive),
                ("@region", region));
        }

        private async Task<List<Product>> ReadAsync(
            string sql,
            CancellationToken ct,
            params (string, object)[] parameters)
        {
            var list = new List<Product>();

            var conn = _conn ?? await GetConnectionAsync(ct);

            await using var cmd = new NpgsqlCommand(sql, conn, _tx);

            foreach (var (n, v) in parameters)
                cmd.Parameters.AddWithValue(n, v ?? DBNull.Value);

            await using var reader = await cmd.ExecuteReaderAsync(ct);

            while (await reader.ReadAsync(ct))
                list.Add(Map(reader));

            return list;
        }

        private static Product Map(NpgsqlDataReader r)
        {
            var stripePriceIdOrdinal = r.GetOrdinal("stripe_price_id");

            return new Product
            {
                Id = r.GetGuid(r.GetOrdinal("id")),
                Code = r.GetString(r.GetOrdinal("code")),
                Name = r.GetString(r.GetOrdinal("name")),
                Description = r.GetString(r.GetOrdinal("description")),
                Type = Enum.Parse<PaymentType>(r.GetString(r.GetOrdinal("type"))),
                Price = r.GetDecimal(r.GetOrdinal("price")),
                Currency = r.GetString(r.GetOrdinal("currency")),
                StripePriceId = r.IsDBNull(stripePriceIdOrdinal)
                    ? throw new InvalidOperationException("StripePriceId is null")
                    : r.GetString(stripePriceIdOrdinal),

                AttributesJson = r.GetString(r.GetOrdinal("attributes")),
                IsActive = r.GetBoolean(r.GetOrdinal("is_active")),
                CreatedAt = r.GetDateTime(r.GetOrdinal("created_at"))
            };
        }
    }
}