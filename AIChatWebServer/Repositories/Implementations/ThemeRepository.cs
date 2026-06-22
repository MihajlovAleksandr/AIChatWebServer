using System.Text.Json;
using AIChatWebServer.Models.Themes;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class ThemeRepository : BaseRepository, IThemeRepository
    {
        private readonly NpgsqlConnection? _conn;
        private readonly NpgsqlTransaction? _tx;
        private readonly IConfiguration _configuration;
        private bool _ownsConnection;

        public ThemeRepository(IConfiguration configuration) : base(configuration) { _configuration = configuration; }

        private ThemeRepository(IConfiguration configuration, NpgsqlConnection conn, NpgsqlTransaction tx) : base(configuration)
        {
            _configuration = configuration;
            _conn = conn;
            _tx = tx;
            _ownsConnection = false;
        }

        public IThemeRepository WithTransaction(NpgsqlConnection conn, NpgsqlTransaction tx)
            => new ThemeRepository(_configuration, conn, tx);

        private async Task<NpgsqlConnection> GetConnectionWithOwnershipAsync(CancellationToken ct)
        {
            if (_conn != null)
                return _conn;

            var conn = await GetConnectionAsync(ct);
            _ownsConnection = true;
            return conn;
        }

        public async Task<Theme?> GetByIdAsync(
            Guid id,
            CancellationToken ct = default)
        {
            var list = await ReadAsync(
                ThemeQueries.GetById,
                ct,
                ("@id", id));

            return list.FirstOrDefault();
        }

        public async Task<Theme?> GetByNameAsync(
            Guid userId,
            string name,
            CancellationToken ct = default)
        {
            var list = await ReadAsync(
                ThemeQueries.GetByName,
                ct,
                ("@userId", userId),
                ("@name", name));

            return list.FirstOrDefault();
        }

        public Task<List<Theme>> GetByUserIdAsync(
            Guid? userId,
            CancellationToken ct = default)
        {
            return ReadAsync(
                ThemeQueries.GetByUserId,
                ct,
                ("@user_id", userId ?? (object)DBNull.Value));
        }

        public Task<List<Theme>> GetByTypeAsync(
            ThemeType type,
            CancellationToken ct = default)
        {
            return ReadAsync(
                ThemeQueries.GetByType,
                ct,
                ("@type", (int)type));
        }

        public Task<List<Theme>> GetAllAsync(
            CancellationToken ct = default)
        {
            return ReadAsync(
                ThemeQueries.GetAll,
                ct);
        }

        public async Task<Guid> CreateAsync(
            Guid? userId,
            string name,
            ThemeType type,
            JsonDocument content,
            CancellationToken ct = default)
        {
            Guid id = Guid.NewGuid();

            var conn = await GetConnectionWithOwnershipAsync(ct);
            await using var cmd = new NpgsqlCommand(ThemeQueries.Create, conn, _tx);

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@user_id", userId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@type", (int)type);
            cmd.Parameters.AddWithValue("@config_json", NpgsqlTypes.NpgsqlDbType.Jsonb, content);

            await cmd.ExecuteNonQueryAsync(ct);

            if (_ownsConnection && _conn == null)
            {
                await conn.DisposeAsync();
            }

            return id;
        }

        public async Task<Theme> UpdateAsync(
            Theme theme,
            CancellationToken ct = default)
        {
            var conn = await GetConnectionWithOwnershipAsync(ct);
            await using var cmd = new NpgsqlCommand(ThemeQueries.Update, conn, _tx);

            cmd.Parameters.AddWithValue("@id", theme.Id);
            cmd.Parameters.AddWithValue("@name", theme.Name);
            cmd.Parameters.AddWithValue("@config_json", NpgsqlTypes.NpgsqlDbType.Jsonb, theme.ConfigJson);
            cmd.Parameters.AddWithValue("@updated_at", DateTime.UtcNow);

            await cmd.ExecuteNonQueryAsync(ct);

            if (_ownsConnection && _conn == null)
            {
                await conn.DisposeAsync();
            }

            return theme;
        }

        public async Task<bool> DeleteAsync(
            Guid id,
            CancellationToken ct = default)
        {
            var conn = await GetConnectionWithOwnershipAsync(ct);
            await using var cmd = new NpgsqlCommand(ThemeQueries.Delete, conn, _tx);

            cmd.Parameters.AddWithValue("@id", id);

            var rowsAffected = await cmd.ExecuteNonQueryAsync(ct);

            if (_ownsConnection && _conn == null)
            {
                await conn.DisposeAsync();
            }

            return rowsAffected > 0;
        }

        public async Task<Theme?> GetSelectedThemeByConnectionIdAsync(
            Guid connectionId,
            CancellationToken ct = default)
        {
            var list = await ReadAsync(
                ThemeQueries.GetSelectedThemeByConnectionId,
                ct,
                ("@connection_id", connectionId));

            return list.FirstOrDefault();
        }

        public async Task UpsertSelectedThemeAsync(
            Guid connectionId,
            Guid themeId,
            CancellationToken ct = default)
        {
            var conn = await GetConnectionWithOwnershipAsync(ct);

            await using var upsertCmd = new NpgsqlCommand(ThemeQueries.UpsertSelectedTheme, conn, _tx);
            upsertCmd.Parameters.AddWithValue("@connection_id", connectionId);
            upsertCmd.Parameters.AddWithValue("@theme_id", themeId);

            await upsertCmd.ExecuteNonQueryAsync(ct);

            if (_ownsConnection && _conn == null)
            {
                await conn.DisposeAsync();
            }
        }

        public async Task<bool> DeleteSelectedThemeAsync(
            Guid connectionId,
            CancellationToken ct = default)
        {
            var conn = await GetConnectionWithOwnershipAsync(ct);
            await using var cmd = new NpgsqlCommand(ThemeQueries.DeleteSelectedTheme, conn, _tx);

            cmd.Parameters.AddWithValue("@connection_id", connectionId);

            var rowsAffected = await cmd.ExecuteNonQueryAsync(ct);

            if (_ownsConnection && _conn == null)
            {
                await conn.DisposeAsync();
            }

            return rowsAffected > 0;
        }

        public async Task<long> GetUsagesCountAsync(
            Guid themeId,
            CancellationToken ct = default)
        {
            var conn = await GetConnectionWithOwnershipAsync(ct);

            await using var cmd = new NpgsqlCommand(ThemeQueries.GetThemeUsageCount, conn, _tx);
            cmd.Parameters.AddWithValue("@theme_id", themeId);

            var count = Convert.ToInt64(await cmd.ExecuteScalarAsync(ct));

            if (_ownsConnection && _conn == null)
            {
                await conn.DisposeAsync();
            }

            return count;
        }

        private async Task<List<Theme>> ReadAsync(
            string sql,
            CancellationToken ct,
            params (string, object)[] parameters)
        {
            var list = new List<Theme>();

            var conn = await GetConnectionWithOwnershipAsync(ct);

            await using var cmd = new NpgsqlCommand(sql, conn, _tx);

            foreach (var (n, v) in parameters)
                cmd.Parameters.AddWithValue(n, v ?? DBNull.Value);

            await using var reader = await cmd.ExecuteReaderAsync(ct);

            while (await reader.ReadAsync(ct))
                list.Add(await MapWithUsageCountAsync(reader, ct));

            if (_ownsConnection && _conn == null)
            {
                await conn.DisposeAsync();
            }

            return list;
        }

        private async Task<Theme> MapWithUsageCountAsync(NpgsqlDataReader r, CancellationToken ct)
        {
            var themeIdOrdinal = r.GetOrdinal("id");
            var userIdOrdinal = r.GetOrdinal("user_id");

            var theme = new Theme
            {
                Id = r.GetGuid(themeIdOrdinal),
                UserId = r.IsDBNull(userIdOrdinal) ? null : r.GetGuid(userIdOrdinal),
                Name = r.GetString(r.GetOrdinal("name")),
                Type = (ThemeType)r.GetInt32(r.GetOrdinal("type")),
                ConfigJson = JsonDocument.Parse(r.GetString(r.GetOrdinal("config_json"))),
                CreatedAt = r.GetDateTime(r.GetOrdinal("created_at")),
                UpdatedAt = r.GetDateTime(r.GetOrdinal("updated_at")),
                UsageCount = await GetUsagesCountAsync(r.GetGuid(themeIdOrdinal), ct)
            };

            return theme;
        }
    }
}