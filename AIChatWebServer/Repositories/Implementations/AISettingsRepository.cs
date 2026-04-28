using AIChatWebServer.Integrations.AI;
using AIChatWebServer.Models.AI;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;
using NpgsqlTypes;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class AISettingsRepository : BaseRepository, IAISettingsRepository
    {
        public async Task<AISettingsModel?> GetByChatId(
            Guid chatId,
            CancellationToken cancellationToken = default)
        {
            await using var conn = await GetConnectionAsync(cancellationToken);
            await using var cmd = new NpgsqlCommand(AISettingsQueries.GetByChatId, conn);

            cmd.Parameters.AddWithValue("@chatId", chatId);

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            if (await reader.ReadAsync(cancellationToken))
            {
                return new AISettingsModel(
                    reader.GetGuid(reader.GetOrdinal("id")),
                    reader.GetGuid(reader.GetOrdinal("chat_id")),
                    reader.IsDBNull(reader.GetOrdinal("custom_prompt"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("custom_prompt")),
                    (AIModel)reader.GetInt32(reader.GetOrdinal("model")),
                    reader.GetDateTime(reader.GetOrdinal("last_update"))
                );
            }

            return null;
        }

        public async Task<AISettingsModel> Add(
            Guid chatId,
            int model,
            string? customPrompt,
            CancellationToken cancellationToken = default)
        {
            await using var conn = await GetConnectionAsync(cancellationToken);

            var id = Guid.NewGuid();

            await using var cmd = new NpgsqlCommand(AISettingsQueries.Insert, conn);

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@chatId", chatId);
            cmd.Parameters.AddWithValue("@model", model);

            if (customPrompt is null)
                cmd.Parameters.AddWithValue("@customPrompt", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@customPrompt", NpgsqlDbType.Text, customPrompt);

            await cmd.ExecuteNonQueryAsync(cancellationToken);

            return new AISettingsModel(
                id,
                chatId,
                customPrompt,
                (AIModel)model,
                DateTime.UtcNow
            );
        }

        public async Task<bool> UpdatePrompt(
            Guid chatId,
            string? customPrompt,
            CancellationToken cancellationToken = default)
        {
            await using var conn = await GetConnectionAsync(cancellationToken);
            await using var cmd = new NpgsqlCommand(AISettingsQueries.UpdatePrompt, conn);

            cmd.Parameters.AddWithValue("@chatId", chatId);

            if (customPrompt is null)
                cmd.Parameters.AddWithValue("@customPrompt", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@customPrompt", NpgsqlDbType.Text, customPrompt);

            var rows = await cmd.ExecuteNonQueryAsync(cancellationToken);

            return rows > 0;
        }
    }
}