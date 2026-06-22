using AIChatWebServer.Models.Payment;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class PaymentItemRepository : BaseRepository, IPaymentItemRepository
    {
        private readonly ILogger<PaymentItemRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly NpgsqlConnection? _conn;
        private readonly NpgsqlTransaction? _tx;
        private readonly bool _isExternalConnection;

        public PaymentItemRepository(IConfiguration configuration,
            ILogger<PaymentItemRepository> logger) : base(configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _isExternalConnection = false;
        }

        private PaymentItemRepository(IConfiguration configuration,
            ILogger<PaymentItemRepository> logger,
            NpgsqlConnection conn,
            NpgsqlTransaction tx) : base(configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _conn = conn;
            _tx = tx;
            _isExternalConnection = true;
        }

        public IPaymentItemRepository WithTransaction(NpgsqlConnection conn, NpgsqlTransaction tx)
            => new PaymentItemRepository(_configuration, _logger, conn, tx);

        public async Task CreateAsync(PaymentItem item, CancellationToken ct = default)
        {
            await ExecuteAsync(
                PaymentItemQueries.Insert,
                ct,
                ("@id", item.Id),
                ("@payment_id", item.PaymentId),
                ("@product_id", item.Product.Id),
                ("@quantity", item.Quantity),
                ("@price", item.Price)
            );
        }

        public async Task<List<PaymentItem>> GetByPaymentAsync(Guid paymentId, string region, CancellationToken ct = default)
        {
            return await ReadAsync(
                PaymentItemQueries.GetByPayment,
                ct,
                ("@payment_id", paymentId),
                ("@region", region));
        }

        public async Task<bool> HasUserPurchasedProductAsync(Guid userId, Guid productId, CancellationToken ct = default)
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

                await using var cmd = new NpgsqlCommand(
                    PaymentItemQueries.HasUserPurchasedProduct,
                    connection,
                    _tx);
                cmd.Parameters.AddWithValue("@user_id", userId);
                cmd.Parameters.AddWithValue("@product_id", productId);
                cmd.Parameters.AddWithValue("@status", PaymentStatuses.Confirmed.ToString().ToUpperInvariant());

                var result = await cmd.ExecuteScalarAsync(ct);
                return result is bool b && b;
            }
            finally
            {
                if (ownsConnection && connection != null)
                {
                    await connection.DisposeAsync();
                }
            }
        }

        private async Task<List<PaymentItem>> ReadAsync(
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
                {
                    cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
                }

                await using var reader = await cmd.ExecuteReaderAsync(ct);

                var list = new List<PaymentItem>();

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

        private static PaymentItem Map(NpgsqlDataReader reader)
        {
            var product = new Product
            {
                Id = reader.GetGuid(reader.GetOrdinal("p_id")),
                Code = reader.GetString(reader.GetOrdinal("code")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Description = reader.GetString(reader.GetOrdinal("description")),
                Type = Enum.Parse<PaymentType>(reader.GetString(reader.GetOrdinal("type"))),
                Price = reader.GetDecimal(reader.GetOrdinal("product_price")),
                Currency = reader.GetString(reader.GetOrdinal("currency")),
                StripePriceId = reader.GetString(reader.GetOrdinal("stripe_price_id")),

                AttributesJson = reader.GetString(reader.GetOrdinal("attributes")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
            };

            return new PaymentItem
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                PaymentId = reader.GetGuid(reader.GetOrdinal("payment_id")),
                Product = product,
                Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                Price = reader.GetDecimal(reader.GetOrdinal("price"))
            };
        }

        private async Task ExecuteAsync(
            string sql,
            CancellationToken ct,
            params (string, object)[] parameters)
        {
            NpgsqlConnection? connection = null;
            NpgsqlTransaction? transaction = null;
            bool ownsConnection = false;
            bool ownsTransaction = false;

            try
            {
                if (_isExternalConnection)
                {
                    connection = _conn;
                    transaction = _tx;
                }
                else
                {
                    connection = await GetConnectionAsync(ct);
                    transaction = await connection.BeginTransactionAsync(ct);
                    ownsConnection = true;
                    ownsTransaction = true;
                }

                await using var cmd = new NpgsqlCommand(sql, connection, transaction);

                foreach (var (name, value) in parameters)
                {
                    cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
                }

                await cmd.ExecuteNonQueryAsync(ct);

                if (ownsTransaction && transaction != null)
                {
                    await transaction.CommitAsync(ct);
                }
            }
            catch (Exception ex)
            {
                if (ownsTransaction && transaction != null)
                {
                    await transaction.RollbackAsync(ct);
                }

                _logger.LogError(ex, "Failed to execute query: {Sql}", sql);
                throw;
            }
            finally
            {
                if (ownsConnection && connection != null)
                {
                    await connection.DisposeAsync();
                }
            }
        }
    }
}