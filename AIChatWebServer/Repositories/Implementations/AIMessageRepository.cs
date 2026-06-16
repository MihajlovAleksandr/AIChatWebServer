using AIChatWebServer.Models.AI;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;
using NpgsqlTypes;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class AIMessageRepository(IConfiguration configuration) : BaseRepository(configuration), IAIMessageRepository
    {
        public async Task<AIMessage> Add(
            Guid chatId,
            AIMessageRole role,
            AIMessageType type,
            string content,
            CancellationToken cancellationToken = default)
        {
            await using var conn = await GetConnectionAsync(cancellationToken);

            var messageId = Guid.NewGuid();

            await using var cmd = new NpgsqlCommand(AIMessageQueries.InsertMessage, conn);

            cmd.Parameters.AddWithValue("@id", messageId);
            cmd.Parameters.AddWithValue("@chatId", chatId);
            cmd.Parameters.AddWithValue("@text", content);
            cmd.Parameters.AddWithValue("@role", NpgsqlDbType.Varchar, role.ToString());
            cmd.Parameters.AddWithValue("@type", NpgsqlDbType.Varchar, type.ToString());

            await cmd.ExecuteNonQueryAsync(cancellationToken);

            return new AIMessage(messageId, chatId, role, type, content);
        }

        public async Task<AIMessage?> GetById(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            await using var conn = await GetConnectionAsync(cancellationToken);
            await using var cmd = new NpgsqlCommand(AIMessageQueries.GetMessageById, conn);

            cmd.Parameters.AddWithValue("@id", id);

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            if (await reader.ReadAsync(cancellationToken))
            {
                var roleString = reader.GetString(reader.GetOrdinal("role"));
                var role = AIMessageRole.ValueFromString(roleString);
                var type = Enum.Parse<AIMessageType>(reader.GetString(reader.GetOrdinal("type")));

                return new AIMessage(
                    reader.GetGuid(reader.GetOrdinal("id")),
                    reader.GetGuid(reader.GetOrdinal("chat_id")),
                    role ?? throw new ArgumentException(),
                    type,
                    reader.GetString(reader.GetOrdinal("text"))
                );
            }

            return null;
        }

        public async Task<IReadOnlyList<AIMessage>> GetByChatId(
            Guid chatId,
            CancellationToken cancellationToken = default)
        {
            await using var conn = await GetConnectionAsync(cancellationToken);
            await using var cmd = new NpgsqlCommand(AIMessageQueries.GetMessagesByChatId, conn);

            cmd.Parameters.AddWithValue("@chatId", chatId);

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            var messages = new List<AIMessage>();

            while (await reader.ReadAsync(cancellationToken))
            {
                var roleString = reader.GetString(reader.GetOrdinal("role"));
                var role = AIMessageRole.ValueFromString(roleString);
                var type = Enum.Parse<AIMessageType>(reader.GetString(reader.GetOrdinal("type")));

                messages.Add(new AIMessage(
                    reader.GetGuid(reader.GetOrdinal("id")),
                    reader.GetGuid(reader.GetOrdinal("chat_id")),
                    role ?? throw new ArgumentException(),
                    type,
                    reader.GetString(reader.GetOrdinal("text"))
                ));
            }

            return messages;
        }

        public async Task<bool> Delete(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            await using var conn = await GetConnectionAsync(cancellationToken);
            await using var cmd = new NpgsqlCommand(AIMessageQueries.DeleteMessage, conn);

            cmd.Parameters.AddWithValue("@id", id);

            var rowsAffected = await cmd.ExecuteNonQueryAsync(cancellationToken);

            return rowsAffected > 0;
        }

        public async Task UseTokens(
            Guid chatId,
            int tokensCount,
            AIModel model,
            TokenOperation operation,
            CancellationToken cancellationToken = default)
        {

            await using var conn = await GetConnectionAsync(cancellationToken);
            await using var cmd = new NpgsqlCommand(AIMessageQueries.UseTokens, conn);

            cmd.Parameters.AddWithValue("@chatId", chatId);
            cmd.Parameters.AddWithValue("@tokensCount", tokensCount);
            cmd.Parameters.AddWithValue("@model", (int)model);
            cmd.Parameters.AddWithValue("@operation", (int)operation);
            cmd.Parameters.AddWithValue("@date", DateTime.UtcNow);

            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }

    }
}