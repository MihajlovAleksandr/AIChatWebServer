using AIChatWebServer.Models.Payment;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class PaymentRepository : BaseRepository, IPaymentRepository
    {
        private readonly NpgsqlConnection? _conn;
        private readonly NpgsqlTransaction? _tx;

        public PaymentRepository() { }

        private PaymentRepository(NpgsqlConnection conn, NpgsqlTransaction tx)
        {
            _conn = conn;
            _tx = tx;
        }

        public IPaymentRepository WithTransaction(NpgsqlConnection conn, NpgsqlTransaction tx)
            => new PaymentRepository(conn, tx);

        public Task CreateAsync(Payment payment, CancellationToken ct = default)
        {
            return ExecuteAsync(
                PaymentQueries.Insert,
                ct,
                ("@id", payment.Id),
                ("@transaction_id", (object?)payment.TransactionId ?? DBNull.Value),
                ("@userId", payment.UserId),
                ("@amount", payment.Amount),
                ("@currency", payment.Currency),
                ("@status", payment.Status)
            );
        }

        public Task ConfirmAsync(Guid paymentId, string transactionId, CancellationToken ct = default)
        {
            return ExecuteAsync(
                PaymentQueries.UpdateStatus,
                ct,
                ("@id", paymentId),
                ("@transaction_id", transactionId),
                ("@status", "CONFIRMED")
            );
        }

        public async Task<Payment?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var list = await ReadAsync(
                PaymentQueries.GetById,
                ct,
                ("@id", id));

            return list.FirstOrDefault();
        }

        public Task<List<Payment>> GetHistoryAsync(Guid userId, CancellationToken ct = default)
        {
            return ReadAsync(
                PaymentQueries.GetByUser,
                ct,
                ("@user_id", userId));
        }

        public async Task<bool> ExistsByTransactionId(string transactionId, CancellationToken ct = default)
        {
            var conn = _conn ?? await GetConnectionAsync(ct);

            await using var cmd = new NpgsqlCommand(
                PaymentQueries.ExistsByTransactionId,
                conn,
                _tx);

            cmd.Parameters.AddWithValue("@transaction_id", transactionId);

            var result = await cmd.ExecuteScalarAsync(ct);

            return result is bool exists && exists;
        }

        private async Task<List<Payment>> ReadAsync(string sql, CancellationToken ct, params (string, object)[] parameters)
        {
            var list = new List<Payment>();

            var conn = _conn ?? await GetConnectionAsync(ct);
            await using var cmd = new NpgsqlCommand(sql, conn, _tx);

            AddParams(cmd, parameters);

            await using var r = await cmd.ExecuteReaderAsync(ct);

            while (await r.ReadAsync(ct))
                list.Add(Map(r));

            return list;
        }

        private async Task ExecuteAsync(string sql, CancellationToken ct, params (string, object)[] parameters)
        {
            var conn = _conn ?? await GetConnectionAsync(ct);
            await using var cmd = new NpgsqlCommand(sql, conn, _tx);

            AddParams(cmd, parameters);
            await cmd.ExecuteNonQueryAsync(ct);
        }

        private static void AddParams(NpgsqlCommand cmd, params (string, object)[] parameters)
        {
            foreach (var (n, v) in parameters)
                cmd.Parameters.AddWithValue(n, v ?? DBNull.Value);
        }

        private static Payment Map(NpgsqlDataReader r)
        {
            return new Payment
            {
                Id = r.GetGuid(r.GetOrdinal("id")),
                TransactionId = r.IsDBNull(r.GetOrdinal("transaction_id"))
                    ? null
                    : r.GetString(r.GetOrdinal("transaction_id")),
                UserId = r.GetGuid(r.GetOrdinal("user_id")),
                Amount = r.GetDecimal(r.GetOrdinal("amount")),
                Currency = r.GetString(r.GetOrdinal("currency")),
                Status = r.GetString(r.GetOrdinal("status")),
                CreatedAt = r.GetDateTime(r.GetOrdinal("created_at"))
            };
        }
    }
}