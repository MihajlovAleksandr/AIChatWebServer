using AIChatWebServer.Models.Links;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;
using System.Data;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class LinkRepository : BaseRepository, ILinkRepository
    {
        public async Task<Guid> CreateAsync(
            string tokenHash,
            LinkType type,
            string payloadJson,
            Guid createdBy,
            DateTime? expiresAt,
            int maxUses,
            CancellationToken ct = default)
        {
            await using var conn =
                await GetConnectionAsync(ct);

            await using var cmd =
                new NpgsqlCommand(LinkQueries.CreateLink, conn);

            Guid id = Guid.NewGuid();

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@tokenHash", tokenHash);
            cmd.Parameters.AddWithValue("@type", type.ToString());
            cmd.Parameters.AddWithValue("@payload", payloadJson);
            cmd.Parameters.AddWithValue("@createdBy", createdBy);
            cmd.Parameters.AddWithValue("@expiresAt",
                expiresAt ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@maxUses", maxUses);

            await cmd.ExecuteNonQueryAsync(ct);

            return id;
        }

        public Task<Link?> GetByTokenHashAsync(
            string tokenHash,
            CancellationToken ct = default) =>
            GetSingleAsync(
                LinkQueries.GetByTokenHash,
                ct,
                ("@tokenHash", tokenHash));

        public Task<Link?> GetByIdAsync(
            Guid id,
            CancellationToken ct = default) =>
            GetSingleAsync(
                LinkQueries.GetById,
                ct,
                ("@id", id));

        public async Task<bool> TryIncrementUsageAsync(
            Guid id,
            CancellationToken ct = default)
        {
            await using var conn =
                await GetConnectionAsync(ct);

            await using var cmd =
                new NpgsqlCommand(
                    LinkQueries.IncrementUsageIfAllowed,
                    conn);

            cmd.Parameters.AddWithValue("@id", id);

            int affected =
                await cmd.ExecuteNonQueryAsync(ct);

            return affected > 0;
        }

        public Task RevokeAsync(
            Guid id,
            CancellationToken ct = default) =>
            ExecuteAsync(
                LinkQueries.Revoke,
                ct,
                ("@id", id));

        public Task DeleteAsync(
            Guid id,
            CancellationToken ct = default) =>
            ExecuteAsync(
                LinkQueries.Delete,
                ct,
                ("@id", id));

        public Task<IReadOnlyList<Link>> GetActiveByCreatorAsync(
            Guid creatorId,
            CancellationToken ct = default) =>
            GetListAsync(
                LinkQueries.GetActiveByCreator,
                ct,
                ("@createdBy", creatorId));

        private async Task<Link?> GetSingleAsync(
            string sql,
            CancellationToken ct,
            params (string, object)[] p)
        {
            var list =
                await ReadAsync(sql, ct, p);

            return list.FirstOrDefault();
        }

        private async Task<IReadOnlyList<Link>> GetListAsync(
            string sql,
            CancellationToken ct,
            params (string, object)[] p)
        {
            return await ReadAsync(sql, ct, p);
        }

        private async Task<List<Link>> ReadAsync(
            string sql,
            CancellationToken ct,
            params (string, object)[] parameters)
        {
            var list =
                new List<Link>();

            await using var conn =
                await GetConnectionAsync(ct);

            await using var cmd =
                new NpgsqlCommand(sql, conn);

            foreach (var (n, v) in parameters)
                cmd.Parameters.AddWithValue(
                    n,
                    v ?? DBNull.Value);

            await using var r =
                await cmd.ExecuteReaderAsync(ct);

            while (await r.ReadAsync(ct))
            {
                list.Add(new Link(
                    r.GetGuid("id"),
                    r.GetString("token_hash"),
                    Enum.Parse<LinkType>(
                        r.GetString("type")),
                    r.GetString("payload"),
                    r.GetGuid("created_by"),
                    r.IsDBNull("expires_at")
                        ? null
                        : r.GetDateTime("expires_at"),
                    r.GetInt32("max_uses"),
                    r.GetInt32("current_uses"),
                    r.GetBoolean("revoked"),
                    r.GetDateTime("created_at")
                ));
            }

            return list;
        }

        private async Task ExecuteAsync(
            string sql,
            CancellationToken ct,
            params (string, object)[] p)
        {
            await using var conn =
                await GetConnectionAsync(ct);

            await using var cmd =
                new NpgsqlCommand(sql, conn);

            foreach (var (n, v) in p)
                cmd.Parameters.AddWithValue(
                    n,
                    v ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync(ct);
        }
    }
}