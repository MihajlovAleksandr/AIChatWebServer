using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Repositories.Models;
using Npgsql;
using System.Data;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class VerificationCodeRepository(
        ILogger<VerificationCodeRepository> logger) :
        BaseRepository,
        IVerificationCodeRepository
    {
        private readonly ILogger<VerificationCodeRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task UpsertAsync(
            Guid userId,
            string type,
            string codeHash,
            DateTime expiresAt,
            CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation(
                    "Upserting verification code for UserId={UserId}, Type={Type}",
                    userId,
                    type);

                await using var connection =
                    await GetConnectionAsync(ct);

                await using var command =
                    new NpgsqlCommand(
                        VerificationCodeQueries.Upsert,
                        connection);

                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@Type", type);
                command.Parameters.AddWithValue("@CodeHash", codeHash);
                command.Parameters.AddWithValue("@ExpiresAt", expiresAt);

                await command.ExecuteNonQueryAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to upsert verification code for UserId={UserId}, Type={Type}",
                    userId,
                    type);

                throw;
            }
        }

        public async Task<VerificationCodeRecord?> GetAsync(
            Guid userId,
            string type,
            CancellationToken ct = default)
        {
            try
            {
                await using var connection =
                    await GetConnectionAsync(ct);

                await using var command =
                    new NpgsqlCommand(
                        VerificationCodeQueries.Get,
                        connection);

                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@Type", type);

                await using var reader =
                    await command.ExecuteReaderAsync(ct);

                if (!await reader.ReadAsync(ct))
                    return null;

                return new VerificationCodeRecord(
                    reader.GetGuid("id"),
                    reader.GetGuid("user_id"),
                    reader.GetString("type"),
                    reader.GetString("code_hash"),
                    reader.GetInt32("attempts"),
                    reader.GetDateTime("expires_at"),
                    reader.GetDateTime("created_at")
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to get verification code for UserId={UserId}, Type={Type}",
                    userId,
                    type);

                throw;
            }
        }

        public async Task IncrementAttemptsAsync(
            Guid id,
            CancellationToken ct = default)
        {
            try
            {
                await using var connection =
                    await GetConnectionAsync(ct);

                await using var command =
                    new NpgsqlCommand(
                        VerificationCodeQueries.IncrementAttempts,
                        connection);

                command.Parameters.AddWithValue("@Id", id);

                await command.ExecuteNonQueryAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to increment attempts for VerificationCodeId={Id}",
                    id);

                throw;
            }
        }

        public async Task DeleteAsync(
            Guid id,
            CancellationToken ct = default)
        {
            try
            {
                await using var connection =
                    await GetConnectionAsync(ct);

                await using var command =
                    new NpgsqlCommand(
                        VerificationCodeQueries.Delete,
                        connection);

                command.Parameters.AddWithValue("@Id", id);

                await command.ExecuteNonQueryAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to delete verification code Id={Id}",
                    id);

                throw;
            }
        }
    }
}
