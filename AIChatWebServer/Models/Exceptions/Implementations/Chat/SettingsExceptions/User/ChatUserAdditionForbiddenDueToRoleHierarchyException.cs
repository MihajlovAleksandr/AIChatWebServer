using AIChatWebServer.Models.Chats;
using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User
{
    public class ChatUserAdditionForbiddenDueToRoleHierarchyException(Guid chatId, Guid userId, Guid addedUserId, ChatUserRole requestedRole) :
        ChatUserSettingsException(
            ChatErrors.ChatUserInvitationForbiddenDueToRoleHierarchy,
            $"User {userId} cannot add a user {addedUserId} with role {requestedRole} in chat {chatId} because their role is lower.");
}
