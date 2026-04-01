using AIChatWebServer.Models.Files;
using AIChatWebServer.Models.Messages;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;
using System.Data;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class MessageRepository : BaseRepository, IMessageRepository
    {
        public async Task<Guid> CreateAsync(
            Guid id,
            Guid chatId,
            Guid userId,
            string text,
            IReadOnlyCollection<Guid> chatUserIds,
            IReadOnlyCollection<Guid> fileIds,
            IReadOnlyCollection<MessageReply> replies,
            CancellationToken ct = default)
        {
            await using var conn = await GetConnectionAsync(ct);
            await using var tx = await conn.BeginTransactionAsync(ct);

            try
            {
                await using (var cmd = new NpgsqlCommand(MessageQueries.Create, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@chatId", chatId);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@text", text);

                    await cmd.ExecuteNonQueryAsync(ct);
                }

                var now = DateTime.UtcNow;

                foreach (var chatUserId in chatUserIds)
                {
                    var status = chatUserId == userId
                        ? MessageStatus.Read
                        : MessageStatus.Sent;

                    await using var cmd = new NpgsqlCommand(MessageQueries.UpsertStatus, conn, tx);

                    cmd.Parameters.AddWithValue("@messageId", id);
                    cmd.Parameters.AddWithValue("@userId", chatUserId);
                    cmd.Parameters.AddWithValue("@status", status.ToString());
                    cmd.Parameters.AddWithValue("@updatedAt", now);

                    await cmd.ExecuteNonQueryAsync(ct);
                }

                foreach (var fileId in fileIds)
                {
                    await using var cmd = new NpgsqlCommand(MessageQueries.InsertMessageFile, conn, tx);

                    cmd.Parameters.AddWithValue("@messageId", id);
                    cmd.Parameters.AddWithValue("@fileId", fileId);

                    await cmd.ExecuteNonQueryAsync(ct);
                }

                foreach (var reply in replies)
                {
                    await using var cmd = new NpgsqlCommand(MessageQueries.InsertReply, conn, tx);

                    cmd.Parameters.AddWithValue("@messageId", id);
                    cmd.Parameters.AddWithValue("@replyMessageId", reply.ReplyMessageId);
                    cmd.Parameters.AddWithValue("@startIndexQuote", (object?)reply.StartIndexQuote ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@endIndexQuote", (object?)reply.EndIndexQuote ?? DBNull.Value);

                    await cmd.ExecuteNonQueryAsync(ct);
                }

                await tx.CommitAsync(ct);

                return id;
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }

        public async Task<Message?> GetById(
            Guid id,
            CancellationToken ct = default)
        {
            await using var conn = await GetConnectionAsync(ct);
            await using var cmd = new NpgsqlCommand(MessageQueries.GetById, conn);

            cmd.Parameters.AddWithValue("@id", id);

            await using var reader = await cmd.ExecuteReaderAsync(ct);

            if (await reader.ReadAsync(ct))
            {
                return MapMessage(reader);
            }

            return null;
        }

        public async Task<Message?> GetByIdWithDependenciesAsync(
            Guid id,
            CancellationToken ct = default)
        {
            var messages = await ReadMessagesAsync(
                MessageQueries.GetById,
                ct,
                ("@id", id));

            return await EnrichAsync(messages, ct)
                .ContinueWith(t => t.Result.FirstOrDefault(), ct);
        }

        public async Task<IReadOnlyList<Message>> GetByChatWithDependenciesAsync(
            Guid chatId,
            CancellationToken ct = default)
        {
            var messages = await ReadMessagesAsync(
                MessageQueries.GetByChat,
                ct,
                ("@chatId", chatId));

            return await EnrichAsync(messages, ct);
        }

        public async Task UpdateTextAsync(
            Guid id,
            string text,
            DateTime updatedAt,
            CancellationToken ct = default)
        {
            await ExecuteAsync(
                MessageQueries.UpdateText,
                ct,
                ("@id", id),
                ("@text", text),
                ("@updatedAt", updatedAt));
        }

        public async Task UpdateStatusAsync(
            Guid messageId,
            Guid userId,
            MessageStatus status,
            DateTime updatedAt,
            CancellationToken ct = default)
        {
            await ExecuteAsync(
                MessageQueries.UpsertStatus,
                ct,
                ("@messageId", messageId),
                ("@userId", userId),
                ("@status", status.ToString()),
                ("@updatedAt", updatedAt));
        }

        public async Task DeleteFileAsync(
            Guid messageId,
            Guid fileId,
            CancellationToken ct = default)
        {
            await ExecuteAsync(
                MessageQueries.DeleteMessageFile,
                ct,
                ("@messageId", messageId),
                ("@fileId", fileId));
        }

        public async Task DeleteMessageAsync(
            Guid id,
            CancellationToken ct = default)
        {
            await ExecuteAsync(
                MessageQueries.DeleteMessage,
                ct,
                ("@id", id));
        }

        private async Task<List<Message>> ReadMessagesAsync(
            string sql,
            CancellationToken ct,
            params (string, object)[] parameters)
        {
            var result = new List<Message>();

            await using var conn = await GetConnectionAsync(ct);
            await using var cmd = new NpgsqlCommand(sql, conn);

            AddParameters(cmd, parameters);

            await using var reader = await cmd.ExecuteReaderAsync(ct);

            while (await reader.ReadAsync(ct))
            {
                result.Add(MapMessage(reader));
            }

            return result;
        }

        private async Task<IReadOnlyList<Message>> EnrichAsync(
            List<Message> messages,
            CancellationToken ct)
        {
            if (messages.Count == 0)
                return messages;

            var ids = messages.Select(x => x.Id).ToArray();

            var statuses = await LoadStatuses(ids, ct);
            var replies = await LoadReplies(ids, ct);
            var files = await LoadFiles(ids, ct);

            return messages.Select(m => new Message(
                m.Id,
                m.ChatId,
                m.UserId,
                m.Text,
                m.Time,
                m.LastUpdate,
                statuses.GetValueOrDefault(m.Id) ?? new Dictionary<Guid, MessageStatus>(),
                replies.GetValueOrDefault(m.Id) ?? new List<MessageReply>(),
                files.GetValueOrDefault(m.Id) ?? new List<FileModel>()
            )).ToList();
        }

        private async Task<Dictionary<Guid, Dictionary<Guid, MessageStatus>>> LoadStatuses(
            Guid[] ids,
            CancellationToken ct)
        {
            var result = new Dictionary<Guid, Dictionary<Guid, MessageStatus>>();

            await using var conn = await GetConnectionAsync(ct);
            await using var cmd = new NpgsqlCommand(MessageQueries.GetStatuses, conn);

            cmd.Parameters.AddWithValue("@ids", ids);

            await using var reader = await cmd.ExecuteReaderAsync(ct);

            while (await reader.ReadAsync(ct))
            {
                var messageId = reader.GetGuid(0);
                var userId = reader.GetGuid(1);
                var status = Enum.Parse<MessageStatus>(reader.GetString(2));

                if (!result.TryGetValue(messageId, out var dict))
                {
                    dict = new Dictionary<Guid, MessageStatus>();
                    result[messageId] = dict;
                }

                dict[userId] = status;
            }

            return result;
        }

        private async Task<Dictionary<Guid, List<MessageReply>>> LoadReplies(
            Guid[] ids,
            CancellationToken ct)
        {
            var result = new Dictionary<Guid, List<MessageReply>>();

            await using var conn = await GetConnectionAsync(ct);
            await using var cmd = new NpgsqlCommand(MessageQueries.GetReplies, conn);

            cmd.Parameters.AddWithValue("@ids", ids);

            await using var reader = await cmd.ExecuteReaderAsync(ct);

            while (await reader.ReadAsync(ct))
            {
                var messageId = reader.GetGuid(0);

                var reply = new MessageReply(
                    reader.GetGuid(1),
                    reader.IsDBNull(2) ? null : reader.GetInt32(2),
                    reader.IsDBNull(3) ? null : reader.GetInt32(3));

                if (!result.TryGetValue(messageId, out var list))
                {
                    list = new List<MessageReply>();
                    result[messageId] = list;
                }

                list.Add(reply);
            }

            return result;
        }

        private async Task<Dictionary<Guid, List<FileModel>>> LoadFiles(
            Guid[] ids,
            CancellationToken ct)
        {
            var result = new Dictionary<Guid, List<FileModel>>();

            await using var conn = await GetConnectionAsync(ct);
            await using var cmd = new NpgsqlCommand(MessageQueries.GetMessageFiles, conn);

            cmd.Parameters.AddWithValue("@ids", ids);

            await using var reader = await cmd.ExecuteReaderAsync(ct);

            while (await reader.ReadAsync(ct))
            {
                var messageId = reader.GetGuid(reader.GetOrdinal("message_id"));

                var file = MapFile(reader);

                if (!result.TryGetValue(messageId, out var list))
                {
                    list = new List<FileModel>();
                    result[messageId] = list;
                }

                list.Add(file);
            }

            return result;
        }

        private static Message MapMessage(IDataRecord r) =>
            new(
                r.GetGuid(r.GetOrdinal("id")),
                r.GetGuid(r.GetOrdinal("chat_id")),
                r.GetGuid(r.GetOrdinal("user_id")),
                r.GetString(r.GetOrdinal("text")),
                r.GetDateTime(r.GetOrdinal("time")),
                r.GetDateTime(r.GetOrdinal("last_update")),
                new Dictionary<Guid, MessageStatus>(),
                new List<MessageReply>(),
                new List<FileModel>()
            );

        private static FileModel MapFile(IDataRecord r) =>
            new(
                r.GetGuid(r.GetOrdinal("id")),
                (FileType)r.GetInt32(r.GetOrdinal("file_type")),
                r.GetString(r.GetOrdinal("file_name")),
                r.GetString(r.GetOrdinal("file_path")),
                r.GetString(r.GetOrdinal("content_type")),
                r.GetInt64(r.GetOrdinal("file_size")),
                r.IsDBNull(r.GetOrdinal("checksum")) ? null : r.GetString(r.GetOrdinal("checksum")),
                r.GetGuid(r.GetOrdinal("uploaded_by")),
                r.GetDateTime(r.GetOrdinal("created_at")),
                r.GetInt32(r.GetOrdinal("reference_count"))
            );

        private static void AddParameters(
            NpgsqlCommand cmd,
            params (string, object)[] parameters)
        {
            foreach (var (name, value) in parameters)
            {
                cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
            }
        }

        private async Task ExecuteAsync(
            string sql,
            CancellationToken ct,
            params (string, object)[] p)
        {
            await using var conn = await GetConnectionAsync(ct);
            await using var cmd = new NpgsqlCommand(sql, conn);

            AddParameters(cmd, p);

            await cmd.ExecuteNonQueryAsync(ct);
        }
    }
}