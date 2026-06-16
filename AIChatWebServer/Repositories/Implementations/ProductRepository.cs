using AIChatWebServer.Models.Payment;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class ProductRepository : BaseRepository, IProductRepository
    {
        private readonly ILogger<ProductRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly NpgsqlConnection? _conn;
        private readonly NpgsqlTransaction? _tx;
        private readonly bool _isExternalConnection;

        public ProductRepository(IConfiguration configuration,
            ILogger<ProductRepository> logger) : base(configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _isExternalConnection = false;
        }

        private ProductRepository(IConfiguration configuration,
            ILogger<ProductRepository> logger,
            NpgsqlConnection conn,
            NpgsqlTransaction tx) : base(configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _conn = conn;
            _tx = tx;
            _isExternalConnection = true;
        }

        public IProductRepository WithTransaction(NpgsqlConnection conn, NpgsqlTransaction tx)
            => new ProductRepository(_configuration, _logger, conn, tx);

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

        public async Task<List<Product>> GetActiveAsync(
            string region,
            CancellationToken ct = default)
        {
            return await ReadAsync(
                ProductQueries.GetActive,
                ct,
                ("@region", region));
        }

        public async Task<List<Product>> GetByTypeAsync(
            string type,
            string region,
            bool onlyActive = true,
            CancellationToken ct = default)
        {
            return await ReadAsync(
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
            NpgsqlConnection? connection = null;
            bool ownsConnection = false;

            try
            {
                if (_isExternalConnection)
                {
                    connection = _conn;
                }
                else
                {
                    connection = await GetConnectionAsync(ct);
                    ownsConnection = true;
                }

                await using var cmd = new NpgsqlCommand(sql, connection, _tx);

                foreach (var (name, value) in parameters)
                    cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);

                await using var reader = await cmd.ExecuteReaderAsync(ct);

                var list = new List<Product>();

                while (await reader.ReadAsync(ct))
                    list.Add(Map(reader));

                return list;
            }
            finally
            {
                if (ownsConnection && connection != null)
                {
                    await connection.DisposeAsync();
                }
            }
        }

        private static Product Map(NpgsqlDataReader reader)
        {
            var stripePriceIdOrdinal = reader.GetOrdinal("stripe_price_id");

            return new Product
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Code = reader.GetString(reader.GetOrdinal("code")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Description = reader.GetString(reader.GetOrdinal("description")),
                Type = Enum.Parse<PaymentType>(reader.GetString(reader.GetOrdinal("type"))),
                Price = reader.GetDecimal(reader.GetOrdinal("price")),
                Currency = reader.GetString(reader.GetOrdinal("currency")),
                StripePriceId = reader.IsDBNull(stripePriceIdOrdinal)
                    ? throw new InvalidOperationException("StripePriceId is null")
                    : reader.GetString(stripePriceIdOrdinal),

                AttributesJson = reader.GetString(reader.GetOrdinal("attributes")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
            };
        }
    }
}