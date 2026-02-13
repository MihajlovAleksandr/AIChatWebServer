using AIChatWebServer.Models.User;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;
using System.Data;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class UserRepository : BaseRepository, IUserRepository
    {
        public async Task<Guid> CreateUserAsync(
            string email,
            string regionCode,
            string languageCode,
            AuthIdentity authIdentity,
            CancellationToken cancellationToken = default)
        {
            await using var conn =
                await GetConnectionAsync(cancellationToken);

            await using var tx =
                await conn.BeginTransactionAsync(cancellationToken);

            try
            {
                Guid userId = Guid.NewGuid();

                await using var createUserCmd =
                    new NpgsqlCommand(UserQueries.CreateUser, conn, tx);

                createUserCmd.Parameters.AddWithValue("@id", userId);
                createUserCmd.Parameters.AddWithValue("@email", email);
                createUserCmd.Parameters.AddWithValue("@regionCode", regionCode);

                await createUserCmd.ExecuteNonQueryAsync(cancellationToken);

                await using var authCmd =
                    new NpgsqlCommand(UserQueries.AddAuthIdentity, conn, tx);

                authCmd.Parameters.AddWithValue("@id", Guid.NewGuid());
                authCmd.Parameters.AddWithValue("@userId", userId);
                authCmd.Parameters.AddWithValue(
                    "@providerCode",
                    authIdentity.Provider.Code);

                authCmd.Parameters.AddWithValue(
                    "@identifier",
                    authIdentity.Identifier);

                authCmd.Parameters.AddWithValue(
                    "@secret",
                    authIdentity.Secret ?? (object)DBNull.Value);

                await authCmd.ExecuteNonQueryAsync(cancellationToken);

                await using var langCmd =
                    new NpgsqlCommand(UserQueries.SaveLanguage, conn, tx);

                langCmd.Parameters.AddWithValue("@id", Guid.NewGuid());
                langCmd.Parameters.AddWithValue("@userId", userId);
                langCmd.Parameters.AddWithValue(
                    "@context",
                    LanguageContext.Account.ToString());

                langCmd.Parameters.AddWithValue(
                    "@languageCode",
                    languageCode);

                await langCmd.ExecuteNonQueryAsync(cancellationToken);

                await using var notificationCmd =
                    new NpgsqlCommand(UserQueries.AddNotifications, conn, tx);

                notificationCmd.Parameters.AddWithValue("@UserId", userId);

                await notificationCmd.ExecuteNonQueryAsync(cancellationToken);

                await tx.CommitAsync(cancellationToken);

                return userId;
            }
            catch (PostgresException ex)
                when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                await tx.RollbackAsync(cancellationToken);
                throw;
            }
            catch
            {
                await tx.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public Task UpdateRegistrationStateAsync(
            Guid userId,
            RegistrationState state,
            CancellationToken ct = default) =>
            ExecuteAsync(
                UserQueries.UpdateRegistrationState,
                ct,
                ("@userId", userId),
                ("@state", (int)state));

        public Task SaveUserDataAsync(
            Guid userId,
            UserData data,
            CancellationToken ct = default) =>
            ExecuteAsync(
                UserQueries.SaveUserData,
                ct,
                ("@id", data.Id),
                ("@userId", userId),
                ("@name", data.Name),
                ("@gender", data.Gender.ToString()),
                ("@age", data.Age));

        public Task SavePreferenceAsync(
            Guid userId,
            Preference pref,
            CancellationToken ct = default) =>
            ExecuteAsync(
                UserQueries.SavePreference,
                ct,
                ("@id", pref.Id),
                ("@userId", userId),
                ("@minAge", pref.MinAge),
                ("@maxAge", pref.MaxAge),
                ("@preferredGender", pref.Gender.ToString()));

        public Task UpdateAsync(
            User user,
            CancellationToken ct = default) =>
            ExecuteAsync(
                UserQueries.UpdateUser,
                ct,
                ("@id", user.Id),
                ("@email", user.Email),
                ("@regionCode", user.RegionCode));

        public Task DeleteAsync(
            Guid userId,
            CancellationToken ct = default) =>
            ExecuteAsync(
                UserQueries.DeleteUser,
                ct,
                ("@userId", userId));

        public async Task<RegistrationState> GetRegistrationStateAsync(
            Guid userId,
            CancellationToken ct = default)
        {
            await using var conn =
                await GetConnectionAsync(ct);

            await using var cmd =
                new NpgsqlCommand(
                    UserQueries.GetRegistrationState,
                    conn);

            cmd.Parameters.AddWithValue("@userId", userId);

            object? result =
                await cmd.ExecuteScalarAsync(ct);

            return (RegistrationState)(int)result!;
        }

        public Task<User?> GetByIdAsync(
            Guid id,
            CancellationToken ct = default) =>
            GetSingleAsync(
                UserQueries.GetUserById,
                ct,
                ("@userId", id));

        public Task<User?> GetByEmailAsync(
            string email,
            CancellationToken ct = default) =>
            GetSingleAsync(
                UserQueries.GetUserByEmail,
                ct,
                ("@email", email));

        public Task<User?> GetByAuthIdentityCodeAsync(
            string provider,
            string identifier,
            CancellationToken ct = default) =>
            GetSingleAsync(
                UserQueries.GetUserByAuthIdentity,
                ct,
                ("@providerCode", provider),
                ("@identifier", identifier));

        public Task<IReadOnlyList<User>> GetByAuthProviderCodeAsync(
            string provider,
            CancellationToken ct = default) =>
            GetListAsync(
                UserQueries.GetUsersByAuthProvider,
                ct,
                ("@providerCode", provider));

        public Task<IReadOnlyList<User>> GetUsersInSameChatsAsync(
            Guid id,
            CancellationToken ct = default) =>
            GetListAsync(
                UserQueries.GetUsersInSameChats,
                ct,
                ("@userId", id));

        public async Task<bool> ExistsByEmailAsync(
            string email,
            CancellationToken ct = default)
        {
            await using var conn =
                await GetConnectionAsync(ct);

            await using var cmd =
                new NpgsqlCommand(
                    UserQueries.ExistsByEmail,
                    conn);

            cmd.Parameters.AddWithValue("@email", email);

            object? result =
                await cmd.ExecuteScalarAsync(ct);

            return (long)result! > 0;
        }

        public async Task<Region> GetRegionByCodeAsync(
            string regionCode,
            CancellationToken ct = default)
        {
            await using var conn =
                await GetConnectionAsync(ct);

            await using var cmd =
                new NpgsqlCommand(
                    UserQueries.GetRegionByCode,
                    conn);

            cmd.Parameters.AddWithValue("@regionCode", regionCode);

            await using var r =
                await cmd.ExecuteReaderAsync(ct);

            if (!await r.ReadAsync(ct))
                throw new InvalidOperationException();

            return new Region
            {
                Code = r.GetString(0),
                Name = r.GetString(1)
            };
        }

        public async Task<UserBan?> GetUserBanByIdAsync(
            Guid userId,
            CancellationToken ct = default)
        {
            await using var conn =
                await GetConnectionAsync(ct);

            await using var cmd =
                new NpgsqlCommand(
                    UserQueries.GetUserBanById,
                    conn);

            cmd.Parameters.AddWithValue("@userId", userId);

            await using var r =
                await cmd.ExecuteReaderAsync(ct);

            if (!await r.ReadAsync(ct))
                return null;

            return new UserBan(
                r.GetGuid(0),
                r.GetGuid(1),
                r.GetString(2),
                Enum.Parse<BanReason>(r.GetString(3)),
                r.GetDateTime(4),
                r.GetDateTime(5));
        }

        private async Task<User?> GetSingleAsync(
            string sql,
            CancellationToken ct,
            params (string, object)[] p)
        {
            var list =
                await ReadUsersAsync(sql, ct, p);

            return list.FirstOrDefault();
        }

        private async Task<IReadOnlyList<User>> GetListAsync(
            string sql,
            CancellationToken ct,
            params (string, object)[] p)
        {
            return await ReadUsersAsync(sql, ct, p);
        }

        private async Task<List<User>> ReadUsersAsync(
            string sql,
            CancellationToken ct,
            params (string, object)[] parameters)
        {
            var users =
                new Dictionary<Guid, User>();

            await using var conn =
                await GetConnectionAsync(ct);

            await using var cmd =
                new NpgsqlCommand(sql, conn);

            foreach (var (n, v) in parameters)
                cmd.Parameters.AddWithValue(n, v);

            await using var r =
                await cmd.ExecuteReaderAsync(ct);

            while (await r.ReadAsync(ct))
            {
                var id = r.GetGuid("user_id");

                if (!users.TryGetValue(id, out var user))
                {
                    user = new User(
                        id,
                        r.GetString("email"),
                        r.GetString("region_code"));

                    typeof(User)
                        .GetProperty("RegistrationState")!
                        .SetValue(
                            user,
                            (RegistrationState)r.GetInt32("registration_state"));

                    users[id] = user;
                }

                if (!r.IsDBNull("user_data_id") && user.UserData == null)
                {
                    user.UserData = new UserData(
                        r.GetGuid("user_data_id"),
                        Enum.Parse<Gender>(
                            r.GetString("user_gender")),
                        r.GetString("user_name"),
                        r.GetInt32("user_age"));
                }

                if (!r.IsDBNull("pref_id") && user.Preference == null)
                {
                    user.Preference = new Preference(
                        r.GetGuid("pref_id"),
                        r.GetInt32("min_age"),
                        r.GetInt32("max_age"),
                        Enum.Parse<PreferenceGender>(
                            r.GetString("preferred_gender")));
                }

                if (!r.IsDBNull("premium_id"))
                {
                    user.Premium.Add(new UserPremium
                    {
                        Id = r.GetGuid("premium_id"),
                        StartTime = r.GetDateTime("start_at"),
                        EndTime = r.GetDateTime("end_at")
                    });
                }

                if (!r.IsDBNull("auth_id"))
                {
                    user.AuthIdentities.Add(
                        new AuthIdentity(
                            r.GetGuid("auth_id"),
                            id,
                            new AuthProvider(
                                r.GetString("provider_code"),
                                r.IsDBNull("provider_description")
                                    ? null
                                    : r.GetString("provider_description")),
                            r.GetString("identifier"),
                            r.IsDBNull("secret")
                                ? null
                                : r.GetString("secret")));
                }

                if (!r.IsDBNull("lang_context"))
                {
                    user.Language[
                        Enum.Parse<LanguageContext>(
                            r.GetString("lang_context"))
                    ] = r.GetString("language_code");
                }
            }

            return [.. users.Values];
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
