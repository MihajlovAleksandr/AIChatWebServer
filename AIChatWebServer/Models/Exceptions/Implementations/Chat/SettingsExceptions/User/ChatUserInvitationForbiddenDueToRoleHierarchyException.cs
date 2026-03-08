using AIChatWebServer.Models.Chats;
using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User
{
    public class ChatUserInvitationForbiddenDueToRoleHierarchyException(Guid chatId, Guid userId, ChatUserRole requestedRole) :
        ChatUserSettingsException(
            ChatErrors.ChatUserInvitationForbiddenDueToRoleHierarchy,
            $"User {userId} cannot invite a user with role {requestedRole} in chat {chatId} because their role is lower.");
}
