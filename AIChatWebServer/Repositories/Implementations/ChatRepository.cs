using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Chats.ValidateSettings;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;
using NpgsqlTypes;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class ChatRepository : BaseRepository, IChatRepository
    {
        public async Task<Guid> CreateAsync(
            ChatType type,
            IDictionary<Guid, string> creatorsWithChatNames,
            CancellationToken cancellationToken = default)
        {
            await using var conn = await GetConnectionAsync(cancellationToken);
            await using var tx = await conn.BeginTransactionAsync(cancellationToken);

            try
            {
                var chatId = Guid.NewGuid();

                await using (var cmd = new NpgsqlCommand(ChatQueries.CreateChat, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@id", chatId);
                    cmd.Parameters.AddWithValue("@type", NpgsqlDbType.Varchar, type.ToString());
                    await cmd.ExecuteNonQueryAsync(cancellationToken);
                }

                var defaultSettings = ChatSettings.CreateDefault();

                await InsertChatSettings(
                    chatId,
                    defaultSettings,
                    conn,
                    tx,
                    cancellationToken);

                foreach (var creatorWithChatName in creatorsWithChatNames)
                {
                    var ownerSettings = UserSettings.CreateOwner();

                    await AddUserInternal(
                        conn,
                        tx,
                        chatId,
                        creatorWithChatName.Key,
                        creatorWithChatName.Value,
                        ownerSettings,
                        cancellationToken);
                }

                await tx.CommitAsync(cancellationToken);

                return chatId;
            }
            catch
            {
                await tx.RollbackAsync(cancellationToken);
                throw;
            }
        }

        private static async Task<Guid> AddOrRestoreUserInternal(
            NpgsqlConnection conn,
            NpgsqlTransaction tx,
            Guid chatId,
            Guid userId,
            string name,
            CancellationToken ct)
        {
            var newUsersChatId = Guid.NewGuid();

            await using var cmd = new NpgsqlCommand(ChatQueries.AddUserToChat, conn, tx);

            cmd.Parameters.AddWithValue("@id", newUsersChatId);
            cmd.Parameters.AddWithValue("@chatId", chatId);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@name", name);

            var result = await cmd.ExecuteScalarAsync(ct);

            return (Guid)result!;
        }

        private static async Task<Guid> AddUserInternal(
            NpgsqlConnection conn,
            NpgsqlTransaction tx,
            Guid chatId,
            Guid userId,
            string name,
            UserSettings settings,
            CancellationToken ct)
        {
            var usersChatId = await AddOrRestoreUserInternal(
                conn,
                tx,
                chatId,
                userId,
                name,
                ct);

            await UpsertUserSettings(
                usersChatId,
                settings,
                conn,
                tx,
                ct);

            return usersChatId;
        }

        private static async Task InsertChatSettings(
            Guid chatId,
            ChatSettings settings,
            NpgsqlConnection conn,
            NpgsqlTransaction tx,
            CancellationToken ct)
        {
            await using var cmd =
                new NpgsqlCommand(ChatQueries.CreateChatSettings, conn, tx);

            cmd.Parameters.AddWithValue("@chatId", chatId);
            cmd.Parameters.AddWithValue("@allowAddByLink", settings.Members.AllowAddByLink);
            cmd.Parameters.AddWithValue("@allowSearchJoin", settings.Members.AllowSearchJoin);
            cmd.Parameters.AddWithValue("@callEnabled", settings.Calls.CallEnabled);
            cmd.Parameters.AddWithValue("@videoEnabled", settings.Calls.VideoEnabled);

            await cmd.ExecuteNonQueryAsync(ct);
        }

        private static async Task UpsertUserSettings(
            Guid usersChatId,
            UserSettings settings,
            NpgsqlConnection conn,
            NpgsqlTransaction tx,
            CancellationToken ct)
        {
            await using var cmd =
                new NpgsqlCommand(ChatQueries.UpsertUserSettings, conn, tx);

            cmd.Parameters.AddWithValue("@usersChatId", usersChatId);
            cmd.Parameters.AddWithValue("@role", NpgsqlDbType.Varchar, settings.Role.ToString());
            cmd.Parameters.AddWithValue("@canAddUsersBySearch", settings.CanAddUsersBySearch);
            cmd.Parameters.AddWithValue("@canAddUserByLink", settings.CanAddUserByLink);
            cmd.Parameters.AddWithValue("@canRemoveUsers", settings.CanRemoveUsers);
            cmd.Parameters.AddWithValue("@canChangeUserSettings", settings.CanChangeUserSettings);
            cmd.Parameters.AddWithValue("@canChangeChatSettings", settings.CanChangeChatSettings);
            cmd.Parameters.AddWithValue("@canStartCalls", settings.CanStartCalls);

            await cmd.ExecuteNonQueryAsync(ct);
        }

        public async Task<Chat?> GetById(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            await using var conn = await GetConnectionAsync(cancellationToken);
            await using var cmd = new NpgsqlCommand(ChatQueries.GetChatById, conn);

            cmd.Parameters.AddWithValue("@chatId", id);

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            Chat? chat = null;

            while (await reader.ReadAsync(cancellationToken))
            {
                if (chat == null)
                {
                    chat = new Chat
                    {
                        Id = reader.GetGuid(reader.GetOrdinal("id")),
                        CreationTime = reader.GetDateTime(reader.GetOrdinal("creation_time")),
                        EndTime = reader.IsDBNull(reader.GetOrdinal("end_time"))
                            ? null
                            : reader.GetDateTime(reader.GetOrdinal("end_time")),
                        Type = Enum.Parse<ChatType>(
                            reader.GetString(reader.GetOrdinal("type"))),
                        Settings = new ChatSettings(
                            new MemberSettings(
                                reader.GetBoolean(reader.GetOrdinal("allow_add_by_link")),
                                reader.GetBoolean(reader.GetOrdinal("allow_search_join"))),
                            new CallSettings(
                                reader.GetBoolean(reader.GetOrdinal("call_enabled")),
                                reader.GetBoolean(reader.GetOrdinal("video_enabled"))))
                    };
                }

                if (!reader.IsDBNull(reader.GetOrdinal("user_id")))
                {
                    var userId = reader.GetGuid(reader.GetOrdinal("user_id"));

                    var roleString = reader.GetString(reader.GetOrdinal("role"));
                    var role = Enum.Parse<ChatUserRole>(roleString);

                    var userSettings = new UserSettings(
                        role,
                        reader.GetBoolean(reader.GetOrdinal("can_add_users_by_search")),
                        reader.GetBoolean(reader.GetOrdinal("can_add_user_by_link")),
                        reader.GetBoolean(reader.GetOrdinal("can_remove_users")),
                        reader.GetBoolean(reader.GetOrdinal("can_change_user_settings")),
                        reader.GetBoolean(reader.GetOrdinal("can_change_chat_settings")),
                        reader.GetBoolean(reader.GetOrdinal("can_start_calls"))
                    );

                    chat.UsersWithData[userId] =
                        new ChatUserData(
                            reader.GetString(reader.GetOrdinal("name")),
                            reader.GetDateTime(reader.GetOrdinal("join_time")),
                            userSettings);
                }
            }

            return chat;
        }

        public async Task AddUser(
            Guid chatId,
            Guid userId,
            string name,
            ChatUserRole role,
            CancellationToken cancellationToken = default)
        {
            await using var conn = await GetConnectionAsync(cancellationToken);
            await using var tx = await conn.BeginTransactionAsync(cancellationToken);

            try
            {
                var settings = UserSettings.Create(role);

                await AddUserInternal(
                    conn,
                    tx,
                    chatId,
                    userId,
                    name,
                    settings,
                    cancellationToken);

                await tx.CommitAsync(cancellationToken);
            }
            catch
            {
                await tx.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public Task RemoveUser(Guid chatId, Guid userId, CancellationToken cancellationToken = default) =>
            ExecuteAsync(ChatQueries.DeleteUserFromChat, cancellationToken,
                ("@chatId", chatId),
                ("@userId", userId));

        public Task UpdateName(Guid chatId, Guid userId, string name, CancellationToken cancellationToken = default) =>
            ExecuteAsync(ChatQueries.UpdateChatName, cancellationToken,
                ("@chatId", chatId),
                ("@userId", userId),
                ("@name", name));

        public Task End(Guid chatId, DateTime endTime, CancellationToken cancellationToken = default) =>
            ExecuteAsync(ChatQueries.EndChat, cancellationToken,
                ("@chatId", chatId),
                ("@endTime", endTime));

        public Task UpdateChatSettings(
            Guid chatId,
            ChatSettings settings,
            CancellationToken cancellationToken = default) =>
            ExecuteAsync(ChatQueries.UpdateChatSettings, cancellationToken,
                ("@chatId", chatId),
                ("@allowAddByLink", settings.Members.AllowAddByLink),
                ("@allowSearchJoin", settings.Members.AllowSearchJoin),
                ("@callEnabled", settings.Calls.CallEnabled),
                ("@videoEnabled", settings.Calls.VideoEnabled));

        public Task UpdateUserSettings(
            Guid chatId,
            Guid userId,
            UserSettings settings,
            CancellationToken cancellationToken = default) =>
            ExecuteAsync(ChatQueries.UpdateUserSettings, cancellationToken,
                ("@chatId", chatId),
                ("@userId", userId),
                ("@role", settings.Role.ToString()),
                ("@canAddUsersBySearch", settings.CanAddUsersBySearch),
                ("@canAddUserByLink", settings.CanAddUserByLink),
                ("@canRemoveUsers", settings.CanRemoveUsers),
                ("@canChangeUserSettings", settings.CanChangeUserSettings),
                ("@canChangeChatSettings", settings.CanChangeChatSettings),
                ("@canStartCalls", settings.CanStartCalls));

        private async Task ExecuteAsync(
            string sql,
            CancellationToken ct,
            params (string, object)[] parameters)
        {
            await using var conn = await GetConnectionAsync(ct);
            await using var cmd = new NpgsqlCommand(sql, conn);

            foreach (var (name, value) in parameters)
                cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync(ct);
        }
    }
}