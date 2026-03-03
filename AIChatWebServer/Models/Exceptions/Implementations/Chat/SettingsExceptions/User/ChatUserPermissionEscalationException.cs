using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User
{
    public sealed class ChatUserPermissionEscalationException(
        Guid chatId,
        Guid actingUserId,
        Guid targetUserId,
        string permissionName)
        : ChatUserSettingsException(
            ChatErrors.ChatUserPermissionEscalationForbidden,
            $"User {actingUserId} cannot grant permission '{permissionName}' to user {targetUserId} in chat {chatId} because it exceeds their own permissions.")
    {
    }
}