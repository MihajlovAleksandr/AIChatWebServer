using AIChatWebServer.Models.Chats;
using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User
{
    public sealed class ChatUserRemovalForbiddenDueToRoleHierarchyException(
        Guid chatId,
        Guid actingUserId,
        ChatUserRole actingRole,
        Guid targetUserId,
        ChatUserRole targetRole)
        : ChatUserSettingsException(
            ChatErrors.ChatUserRemovalForbiddenDueToRoleHierarchy,
            $"User {actingUserId} with role '{actingRole}' cannot remove user {targetUserId} with higher role '{targetRole}' in chat {chatId}.");
}
