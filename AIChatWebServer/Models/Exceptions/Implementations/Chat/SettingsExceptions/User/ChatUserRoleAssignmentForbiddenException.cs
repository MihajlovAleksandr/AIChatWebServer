using AIChatWebServer.Models.Chats;
using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User
{
    public sealed class ChatUserRoleAssignmentForbiddenException(
        Guid chatId,
        Guid actingUserId,
        ChatUserRole actingRole,
        ChatUserRole attemptedRole)
        : ChatUserSettingsException(
            ChatErrors.ChatUserRoleAssignmentForbidden,
            $"User {actingUserId} with role '{actingRole}' cannot assign role '{attemptedRole}' in chat {chatId} because it is higher than their own.")
    {
    }
}