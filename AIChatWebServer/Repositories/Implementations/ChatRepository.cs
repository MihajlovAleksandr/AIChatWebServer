using AIChatWebServer.Models.AI;
using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Chats.ValidateSettings;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Repositories.Models;
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

            cmd.Parameters.AddWithValue("@messageFilesEnabled", settings.Messages.MessageFilesEnabled);
            cmd.Parameters.AddWithValue("@messageImagesEnabled", settings.Messages.MessageImagesEnabled);
            cmd.Parameters.AddWithValue("@voiceMessageEnabled", settings.Messages.VoiceMessageEnabled);
            cmd.Parameters.AddWithValue("@videoMessageEnabled", settings.Messages.VideoMessageEnabled);

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

            cmd.Parameters.AddWithValue("@messagesEnabled", settings.Messages.MessagesEnabled);
            cmd.Parameters.AddWithValue("@messageFilesEnabled", settings.Messages.MessageFilesEnabled);
            cmd.Parameters.AddWithValue("@messageImagesEnabled", settings.Messages.MessageImagesEnabled);
            cmd.Parameters.AddWithValue("@voiceMessageEnabled", settings.Messages.VoiceMessageEnabled);
            cmd.Parameters.AddWithValue("@videoMessageEnabled", settings.Messages.VideoMessageEnabled);
            cmd.Parameters.AddWithValue("@editMessagesEnabled", settings.Messages.EditMessagesEnabled);
            cmd.Parameters.AddWithValue("@deleteOwnMessagesEnabled", settings.Messages.DeleteOwnMessagesEnabled);
            cmd.Parameters.AddWithValue("@deleteOtherMessagesEnabled", settings.Messages.DeleteOtherMessagesEnabled);

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
                                reader.GetBoolean(reader.GetOrdinal("video_enabled"))),

                            new MessageSettings(
                                reader.GetBoolean(reader.GetOrdinal("message_files_enabled")),
                                reader.GetBoolean(reader.GetOrdinal("message_images_enabled")),
                                reader.GetBoolean(reader.GetOrdinal("voice_message_enabled")),
                                reader.GetBoolean(reader.GetOrdinal("video_message_enabled")))
                        )
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
                        reader.GetBoolean(reader.GetOrdinal("can_start_calls")),
                        new UserMessageSettings(
                            reader.GetBoolean(reader.GetOrdinal("messages_enabled")),
                            reader.GetBoolean(reader.GetOrdinal("message_files_enabled")),
                            reader.GetBoolean(reader.GetOrdinal("message_images_enabled")),
                            reader.GetBoolean(reader.GetOrdinal("voice_message_enabled")),
                            reader.GetBoolean(reader.GetOrdinal("video_message_enabled")),
                            reader.GetBoolean(reader.GetOrdinal("edit_messages_enabled")),
                            reader.GetBoolean(reader.GetOrdinal("delete_own_messages_enabled")),
                            reader.GetBoolean(reader.GetOrdinal("delete_other_messages_enabled"))
                        )
                    );

                    chat.UsersWithData[userId] =
                        new ChatUserData(
                            reader.GetGuid(reader.GetOrdinal("user_chat_id")),
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
                ("@videoEnabled", settings.Calls.VideoEnabled),
                ("@messageFilesEnabled", settings.Messages.MessageFilesEnabled),
                ("@messageImagesEnabled", settings.Messages.MessageImagesEnabled),
                ("@voiceMessageEnabled", settings.Messages.VoiceMessageEnabled),
                ("@videoMessageEnabled", settings.Messages.VideoMessageEnabled));

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
                ("@canStartCalls", settings.CanStartCalls),
                ("@messagesEnabled", settings.Messages.MessagesEnabled),
                ("@messageFilesEnabled", settings.Messages.MessageFilesEnabled),
                ("@messageImagesEnabled", settings.Messages.MessageImagesEnabled),
                ("@voiceMessageEnabled", settings.Messages.VoiceMessageEnabled),
                ("@videoMessageEnabled", settings.Messages.VideoMessageEnabled),
                ("@editMessagesEnabled", settings.Messages.EditMessagesEnabled),
                ("@deleteOwnMessagesEnabled", settings.Messages.DeleteOwnMessagesEnabled),
                ("@deleteOtherMessagesEnabled", settings.Messages.DeleteOtherMessagesEnabled));

        public async Task<IReadOnlyList<Chat>> GetByUserId(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            await using var conn = await GetConnectionAsync(cancellationToken);
            await using var cmd = new NpgsqlCommand(ChatQueries.GetChatsByUserId, conn);

            cmd.Parameters.AddWithValue("@userId", userId);

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            var chats = new Dictionary<Guid, Chat>();

            while (await reader.ReadAsync(cancellationToken))
            {
                var chatId = reader.GetGuid(reader.GetOrdinal("id"));

                if (!chats.TryGetValue(chatId, out var chat))
                {
                    chat = new Chat
                    {
                        Id = chatId,
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
                                reader.GetBoolean(reader.GetOrdinal("video_enabled"))),

                            new MessageSettings(
                                reader.GetBoolean(reader.GetOrdinal("message_files_enabled")),
                                reader.GetBoolean(reader.GetOrdinal("message_images_enabled")),
                                reader.GetBoolean(reader.GetOrdinal("voice_message_enabled")),
                                reader.GetBoolean(reader.GetOrdinal("video_message_enabled")))
                        )
                    };

                    chats[chatId] = chat;
                }

                if (!reader.IsDBNull(reader.GetOrdinal("user_id")))
                {
                    var uId = reader.GetGuid(reader.GetOrdinal("user_id"));

                    var role = Enum.Parse<ChatUserRole>(
                        reader.GetString(reader.GetOrdinal("role")));

                    var userSettings = new UserSettings(
                        role,
                        reader.GetBoolean(reader.GetOrdinal("can_add_users_by_search")),
                        reader.GetBoolean(reader.GetOrdinal("can_add_user_by_link")),
                        reader.GetBoolean(reader.GetOrdinal("can_remove_users")),
                        reader.GetBoolean(reader.GetOrdinal("can_change_user_settings")),
                        reader.GetBoolean(reader.GetOrdinal("can_change_chat_settings")),
                        reader.GetBoolean(reader.GetOrdinal("can_start_calls")),
                        new UserMessageSettings(
                            reader.GetBoolean(reader.GetOrdinal("messages_enabled")),
                            reader.GetBoolean(reader.GetOrdinal("message_files_enabled")),
                            reader.GetBoolean(reader.GetOrdinal("message_images_enabled")),
                            reader.GetBoolean(reader.GetOrdinal("voice_message_enabled")),
                            reader.GetBoolean(reader.GetOrdinal("video_message_enabled")),
                            reader.GetBoolean(reader.GetOrdinal("edit_messages_enabled")),
                            reader.GetBoolean(reader.GetOrdinal("delete_own_messages_enabled")),
                            reader.GetBoolean(reader.GetOrdinal("delete_other_messages_enabled"))
                        )
                    );

                    chat.UsersWithData[uId] = new ChatUserData(
                        reader.GetGuid(reader.GetOrdinal("user_chat_id")),
                        reader.GetString(reader.GetOrdinal("name")),
                        reader.GetDateTime(reader.GetOrdinal("join_time")),
                        userSettings);
                }
            }

            return chats.Values.ToList();
        }


        public async Task<SyncChatsResult> GetChangesAsync(
            Guid userId,
            DateTime since,
            CancellationToken ct)
        {
            var createdIds = new HashSet<Guid>();
            var updatedIds = new HashSet<Guid>();
            var deletedIds = new HashSet<Guid>();

            await using var conn = await GetConnectionAsync(ct);
            await using var cmd = new NpgsqlCommand(ChatQueries.GetChatChanges, conn);

            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@since", since);

            await using var reader = await cmd.ExecuteReaderAsync(ct);

            while (await reader.ReadAsync(ct))
            {
                var type = reader.GetString(0);
                var id = reader.GetGuid(1);

                switch (type)
                {
                    case "created":
                        createdIds.Add(id);
                        break;

                    case "updated":
                        if (!createdIds.Contains(id))
                            updatedIds.Add(id);
                        break;

                    case "deleted":
                        createdIds.Remove(id);
                        updatedIds.Remove(id);
                        deletedIds.Add(id);
                        break;
                }
            }

            var created = await LoadChats(createdIds, ct);
            var updated = await LoadChats(updatedIds, ct);

            return new SyncChatsResult(
                created,
                updated,
                deletedIds.ToList()
            );
        }
        
        private async Task<IReadOnlyList<Chat>> LoadChats(
            IReadOnlyCollection<Guid> ids,
            CancellationToken ct)
        {
            var result = new List<Chat>();

            foreach (var id in ids)
            {
                var chat = await GetById(id, ct);
                if (chat != null)
                    result.Add(chat);
            }

            return result;
        }

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