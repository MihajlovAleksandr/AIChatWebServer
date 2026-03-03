using AIChatWebServer.Models.Chats;
using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User
{
    public sealed class ChatUserSettingsChangeForbiddenDueToRoleHierarchyException(
        Guid chatId,
        Guid actingUserId,
        ChatUserRole actingRole,
        Guid targetUserId,
        ChatUserRole targetRole)
        : ChatUserSettingsException(
            ChatErrors.ChatUserSettingsChangeForbiddenDueToRoleHierarchy,
            $"User {actingUserId} with role '{actingRole}' cannot change settings of user {targetUserId} with role '{targetRole}' in chat {chatId}.")
    {
    }
}