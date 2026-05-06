using AIChatWebServer.Models.User;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class UserPremiumRepository : BaseRepository, IUserPremiumRepository
    {
        private readonly NpgsqlConnection? _conn;
        private readonly NpgsqlTransaction? _tx;

        public UserPremiumRepository() { }

        private UserPremiumRepository(NpgsqlConnection conn, NpgsqlTransaction tx)
        {
            _conn = conn;
            _tx = tx;
        }

        public IUserPremiumRepository WithTransaction(NpgsqlConnection conn, NpgsqlTransaction tx)
            => new UserPremiumRepository(conn, tx);

        public Task CreateAsync(
            Guid userId,
            Guid paymentItemId,
            DateTime startAt,
            DateTime endAt,
            bool isAutoRenew = false,
            string? subscriptionId = null,
            CancellationToken ct = default)
        {
            return ExecuteAsync(
                UserPremiumQueries.Create,
                ct,
                ("@id", Guid.NewGuid()),
                ("@user_id", userId),
                ("@payment_item_id", paymentItemId),
                ("@start_at", startAt),
                ("@end_at", endAt),
                ("@is_auto_renew", isAutoRenew),
                ("@subscription_id", (object?)subscriptionId ?? DBNull.Value));
        }

        public async Task<UserPremium?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var list = await ReadAsync(
                UserPremiumQueries.GetById,
                ct,
                ("@id", id));

            return list.FirstOrDefault();
        }

        public Task<List<UserPremium>> GetHistoryAsync(Guid userId, CancellationToken ct = default)
        {
            return ReadAsync(
                UserPremiumQueries.GetByUser,
                ct,
                ("@user_id", userId));
        }

        public async Task<UserPremium?> GetActiveAsync(Guid userId, CancellationToken ct = default)
        {
            var list = await ReadAsync(
                UserPremiumQueries.GetActive,
                ct,
                ("@user_id", userId));

            return list.FirstOrDefault();
        }

        public async Task<UserPremium?> GetLastAsync(Guid userId, CancellationToken ct = default)
        {
            var list = await ReadAsync(
                UserPremiumQueries.GetLast,
                ct,
                ("@user_id", userId));

            return list.FirstOrDefault();
        }

        public async Task<Guid?> GetUserIdBySubscriptionIdAsync(
            string subscriptionId,
            CancellationToken ct = default)
        {
            var conn = _conn ?? await GetConnectionAsync(ct);

            await using var cmd = new NpgsqlCommand(
                UserPremiumQueries.GetUserIdBySubscriptionId,
                conn,
                _tx);

            cmd.Parameters.AddWithValue("@subscription_id", subscriptionId);

            var result = await cmd.ExecuteScalarAsync(ct);

            if (result == null || result == DBNull.Value)
                return null;

            return (Guid)result;
        }

        public async Task<UserPremium?> GetAutoRenewAsync(Guid userId, CancellationToken ct = default)
        {
            var list = await ReadAsync(
                           UserPremiumQueries.GetAutoRenew,
                           ct,
                           ("@userId", userId));

            return list.FirstOrDefault();
        }

        public Task CancelAutoRenew(string subscriptionId, CancellationToken ct = default)
        {
            return ExecuteAsync(
                UserPremiumQueries.CancelAutoRenew,
                ct,
                ("@subscriptionId", subscriptionId));
        }

        private async Task<List<UserPremium>> ReadAsync(string sql, CancellationToken ct, params (string, object)[] parameters)
        {
            var list = new List<UserPremium>();

            var conn = _conn ?? await GetConnectionAsync(ct);
            await using var cmd = new NpgsqlCommand(sql, conn, _tx);

            foreach (var (n, v) in parameters)
                cmd.Parameters.AddWithValue(n, v ?? DBNull.Value);

            await using var r = await cmd.ExecuteReaderAsync(ct);

            while (await r.ReadAsync(ct))
                list.Add(Map(r));

            return list;
        }

        private async Task ExecuteAsync(string sql, CancellationToken ct, params (string, object)[] parameters)
        {
            var conn = _conn ?? await GetConnectionAsync(ct);
            await using var cmd = new NpgsqlCommand(sql, conn, _tx);

            foreach (var (n, v) in parameters)
                cmd.Parameters.AddWithValue(n, v ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync(ct);
        }

        private static UserPremium Map(NpgsqlDataReader r)
        {
            return new UserPremium
            {
                Id = r.GetGuid(r.GetOrdinal("id")),
                StartTime = r.GetDateTime(r.GetOrdinal("start_at")),
                EndTime = r.GetDateTime(r.GetOrdinal("end_at")),
                IsAutoRenew = r.GetBoolean(r.GetOrdinal("is_auto_renew")),
                SubscriptionId = r.IsDBNull(r.GetOrdinal("subscription_id"))
                    ? null
                    : r.GetString(r.GetOrdinal("subscription_id"))
            };
        }
    }
}