using AIChatWebServer.Models.User;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class UserDeletionRepository : BaseRepository, IUserDeletionRepository
    {
        public async Task<bool> DeleteUser(
            Guid userId,
            Guid deletedUserId,
            CancellationToken cancellationToken = default)
        {
            await using var conn = await GetConnectionAsync(cancellationToken);
            await using var tx = await conn.BeginTransactionAsync(cancellationToken);

            try
            {
                await ExecuteNoParams(conn, tx, UserDeletionQueries.EnsureDeletedAccount, deletedUserId, cancellationToken);

                await ExecuteUser(conn, tx, UserDeletionQueries.DeleteConnections, userId, cancellationToken);
                await ExecuteUser(conn, tx, UserDeletionQueries.DeleteVerificationCodes, userId, cancellationToken);
                await ExecuteUser(conn, tx, UserDeletionQueries.DeleteAuthIdentities, userId, cancellationToken);
                await ExecuteUser(conn, tx, UserDeletionQueries.DeleteUploadSessions, userId, cancellationToken);
                await ExecuteUser(conn, tx, UserDeletionQueries.DeleteMatchmaking, userId, cancellationToken);
                await ExecuteUser(conn, tx, UserDeletionQueries.DeleteGroupChatSearch, userId, cancellationToken);
                await ExecuteUser(conn, tx, UserDeletionQueries.DeleteUserData, userId, cancellationToken);
                await ExecuteUser(conn, tx, UserDeletionQueries.DeletePreferences, userId, cancellationToken);
                await ExecuteUser(conn, tx, UserDeletionQueries.DeleteUserLanguages, userId, cancellationToken);
                await ExecuteUser(conn, tx, UserDeletionQueries.DeleteNotifications, userId, cancellationToken);
                await ExecuteUser(conn, tx, UserDeletionQueries.DeleteMessageStatuses, userId, cancellationToken);
                await ExecuteUser(conn, tx, UserDeletionQueries.DeleteUserChatSettings, userId, cancellationToken);

                await ExecuteUser(conn, tx, UserDeletionQueries.SoftDeleteUsersChats, userId, cancellationToken);

                await ExecuteAnon(conn, tx, UserDeletionQueries.AnonMessages, userId, deletedUserId, cancellationToken);
                await ExecuteAnon(conn, tx, UserDeletionQueries.AnonPayments, userId, deletedUserId, cancellationToken);
                await ExecuteAnon(conn, tx, UserDeletionQueries.AnonPremium, userId, deletedUserId, cancellationToken);
                await ExecuteAnon(conn, tx, UserDeletionQueries.AnonReports, userId, deletedUserId, cancellationToken);
                await ExecuteAnon(conn, tx, UserDeletionQueries.AnonBans, userId, deletedUserId, cancellationToken);
                await ExecuteAnon(conn, tx, UserDeletionQueries.AnonFiles, userId, deletedUserId, cancellationToken);
                await ExecuteAnon(conn, tx, UserDeletionQueries.AnonLinks, userId, deletedUserId, cancellationToken);

                await ExecuteUser(conn, tx, UserDeletionQueries.UpdateAdmins, userId, cancellationToken);

                var affected = await ExecuteUser(conn, tx, UserDeletionQueries.FinalizeUser, userId, cancellationToken);

                await tx.CommitAsync(cancellationToken);

                return affected > 0;
            }
            catch
            {
                await tx.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task<UserMeta?> GetUserMeta(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            await using var conn = await GetConnectionAsync(cancellationToken);
            await using var cmd = new NpgsqlCommand(UserDeletionQueries.GetUserMeta, conn);

            cmd.Parameters.AddWithValue("@userId", userId);

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            if (await reader.ReadAsync(cancellationToken))
            {
                return new UserMeta(
                    reader.GetGuid(reader.GetOrdinal("id")),
                    reader.GetBoolean(reader.GetOrdinal("deleted_status"))
                );
            }

            return null;
        }

        private static async Task<int> ExecuteNoParams(
            NpgsqlConnection conn,
            NpgsqlTransaction tx,
            string sql,
            Guid deletedUserId,
            CancellationToken cancellationToken)
        {
            await using var cmd = new NpgsqlCommand(sql, conn, tx);
            cmd.Parameters.AddWithValue("@deletedUserId", deletedUserId);
            return await cmd.ExecuteNonQueryAsync(cancellationToken);
        }

        private static async Task<int> ExecuteUser(
            NpgsqlConnection conn,
            NpgsqlTransaction tx,
            string sql,
            Guid userId,
            CancellationToken cancellationToken)
        {
            await using var cmd = new NpgsqlCommand(sql, conn, tx);
            cmd.Parameters.AddWithValue("@userId", userId);
            return await cmd.ExecuteNonQueryAsync(cancellationToken);
        }

        private static async Task<int> ExecuteAnon(
            NpgsqlConnection conn,
            NpgsqlTransaction tx,
            string sql,
            Guid userId,
            Guid deletedUserId,
            CancellationToken cancellationToken)
        {
            await using var cmd = new NpgsqlCommand(sql, conn, tx);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@deletedUserId", deletedUserId);
            return await cmd.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}