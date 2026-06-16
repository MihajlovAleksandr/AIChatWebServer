using AIChatWebServer.Models.Payment;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class PaymentRepository : BaseRepository, IPaymentRepository
    {
        private readonly ILogger<PaymentRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly NpgsqlConnection? _conn;
        private readonly NpgsqlTransaction? _tx;
        private readonly bool _isExternalConnection;

        public PaymentRepository(IConfiguration configuration,
            ILogger<PaymentRepository> logger) : base(configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _isExternalConnection = false;
        }

        private PaymentRepository(IConfiguration configuration,
            ILogger<PaymentRepository> logger,
            NpgsqlConnection conn,
            NpgsqlTransaction tx) : base(configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _conn = conn;
            _tx = tx;
            _isExternalConnection = true;
        }

        public IPaymentRepository WithTransaction(
            NpgsqlConnection conn,
            NpgsqlTransaction tx)
        {
            return new PaymentRepository(_configuration, _logger, conn, tx);
        }

        public async Task CreateAsync(
            Payment payment,
            CancellationToken ct = default)
        {
            await ExecuteAsync(
                PaymentQueries.Insert,
                ct,
                ("@id", payment.Id),
                ("@transaction_id", (object?)payment.TransactionId ?? DBNull.Value),
                ("@stripe_charge_id", (object?)payment.StripeChargeId ?? DBNull.Value),
                ("@userId", payment.UserId),
                ("@amount", payment.Amount),
                ("@currency", payment.Currency.ToUpperInvariant()),
                ("@stripe_invoice_url", (object?)payment.StripeInvoiceUrl ?? DBNull.Value),
                ("@status", payment.Status.ToString().ToUpperInvariant())
            );
        }

        public async Task ConfirmAsync(
            Guid paymentId,
            string transactionId,
            string? stripeChargeId,
            string? stripeInvoiceUrl,
            CancellationToken ct = default)
        {
            await ExecuteAsync(
                PaymentQueries.UpdateStatus,
                ct,
                ("@id", paymentId),
                ("@transaction_id", transactionId),
                ("@stripe_charge_id", (object?)stripeChargeId ?? DBNull.Value),
                ("@stripe_invoice_url", (object?)stripeInvoiceUrl ?? DBNull.Value),
                ("@status", PaymentStatuses.Confirmed.ToString().ToUpperInvariant())
            );
        }

        public async Task FailAsync(
            Guid paymentId,
            string? transactionId = null,
            string? stripeChargeId = null,
            string? stripeInvoiceUrl = null,
            CancellationToken ct = default)
        {
            await ExecuteAsync(
                PaymentQueries.UpdateStatus,
                ct,
                ("@id", paymentId),
                ("@transaction_id", (object?)transactionId ?? DBNull.Value),
                ("@stripe_charge_id", (object?)stripeChargeId ?? DBNull.Value),
                ("@stripe_invoice_url", (object?)stripeInvoiceUrl ?? DBNull.Value),
                ("@status", PaymentStatuses.Failed.ToString().ToUpperInvariant())
            );
        }

        public async Task UpdateReceiptUrlAsync(
            string stripeChargeId,
            string stripeInvoiceUrl,
            CancellationToken ct = default)
        {
            await ExecuteAsync(
                PaymentQueries.UpdateReceiptUrl,
                ct,
                ("@stripe_charge_id", stripeChargeId),
                ("@stripe_invoice_url", stripeInvoiceUrl)
            );
        }

        public async Task<Payment?> GetByIdAsync(
            Guid id,
            CancellationToken ct = default)
        {
            var list = await ReadAsync(
                PaymentQueries.GetById,
                ct,
                ("@id", id));

            return list.FirstOrDefault();
        }

        public async Task<Payment?> GetByStripeChargeIdAsync(
            string stripeChargeId,
            CancellationToken ct = default)
        {
            var list = await ReadAsync(
                PaymentQueries.GetByStripeChargeId,
                ct,
                ("@stripe_charge_id", stripeChargeId));

            return list.FirstOrDefault();
        }

        public async Task<List<Payment>> GetHistoryAsync(
            Guid userId,
            CancellationToken ct = default)
        {
            return await ReadAsync(
                PaymentQueries.GetByUser,
                ct,
                ("@user_id", userId));
        }

        public async Task<bool> ExistsByTransactionId(
            string transactionId,
            CancellationToken ct = default)
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
                    PaymentQueries.ExistsByTransactionId,
                    connection,
                    _tx);

                cmd.Parameters.AddWithValue("@transaction_id", transactionId);

                var result = await cmd.ExecuteScalarAsync(ct);

                return result is bool exists && exists;
            }
            finally
            {
                if (ownsConnection && connection != null)
                {
                    await connection.DisposeAsync();
                }
            }
        }

        public async Task<Payment?> GetByPremiumIdAsync(
            Guid premiumId,
            CancellationToken ct = default)
        {
            var list = await ReadAsync(
                PaymentQueries.GetByPremiumId,
                ct,
                ("@premium_id", premiumId));

            return list.FirstOrDefault();
        }

        public async Task ExpirePendingPaymentsAsync(
            DateTime expiredBeforeUtc,
            CancellationToken ct = default)
        {
            await ExecuteAsync(
                PaymentQueries.ExpirePendingPayments,
                ct,
                (
                    "@expired_status",
                    PaymentStatuses.Expired.ToString().ToUpperInvariant()
                ),
                (
                    "@pending_status",
                    PaymentStatuses.Pending.ToString().ToUpperInvariant()
                ),
                (
                    "@expired_before",
                    expiredBeforeUtc
                )
            );
        }

        private async Task<List<Payment>> ReadAsync(
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

                AddParams(cmd, parameters);

                await using var reader = await cmd.ExecuteReaderAsync(ct);

                var list = new List<Payment>();

                while (await reader.ReadAsync(ct))
                {
                    list.Add(Map(reader));
                }

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

                AddParams(cmd, parameters);

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

        private static void AddParams(
            NpgsqlCommand cmd,
            params (string, object)[] parameters)
        {
            foreach (var (name, value) in parameters)
            {
                cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
            }
        }

        private static Payment Map(NpgsqlDataReader reader)
        {
            var statusValue = reader.GetString(reader.GetOrdinal("status"));

            if (!Enum.TryParse<PaymentStatuses>(
                    statusValue,
                    true,
                    out var status))
            {
                throw new InvalidOperationException(
                    $"Unknown payment status: {statusValue}");
            }

            return new Payment
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),

                TransactionId = reader.IsDBNull(reader.GetOrdinal("transaction_id"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("transaction_id")),

                StripeChargeId = reader.IsDBNull(reader.GetOrdinal("stripe_charge_id"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("stripe_charge_id")),

                UserId = reader.GetGuid(reader.GetOrdinal("user_id")),

                Amount = reader.GetDecimal(reader.GetOrdinal("amount")),

                Currency = reader.GetString(reader.GetOrdinal("currency")),

                StripeInvoiceUrl = reader.IsDBNull(reader.GetOrdinal("stripe_invoice_url"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("stripe_invoice_url")),

                Status = status,

                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
            };
        }
    }
}