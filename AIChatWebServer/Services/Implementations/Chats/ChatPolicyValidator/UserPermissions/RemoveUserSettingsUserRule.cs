using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.UserPermissions
{
    public class RemoveUserSettingsUserRule
        : ConversationActionRuleBase<RemoveUserAction>
    {
        public override void Validate(Chat chat, RemoveUserAction action)
        {
            if (!chat.UsersWithData.TryGetValue(action.UserId, out ChatUserData? chatUserData))
            {
                throw new UserDoesNotBelongToChatException(chat.Id, action.UserId);
            }

            if (action.UserId.Equals(action.RemovedUserId))
                return;

            if (!chat.UsersWithData.TryGetValue(action.RemovedUserId, out ChatUserData? removedChatUserData))
            {
                throw new UserNotInChatException(chat.Id, action.RemovedUserId);
            }

            if (!chatUserData.UserSettings.CanRemoveUsers)
            {
                throw new ChatUserRemovalDisabledByUserSettingsException(chat.Id, action.UserId);
            }

            if (!chatUserData.UserSettings.IsHigherThan(removedChatUserData.UserSettings))
            {
                throw new ChatUserRemovalForbiddenDueToRoleHierarchyException(
                    chat.Id, action.UserId,
                    chatUserData.UserSettings.Role,
                    action.RemovedUserId,
                    removedChatUserData.UserSettings.Role);
            }
        }
    }
}
