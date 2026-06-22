using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.UserPermissions
{
    public class InviteUserSettingsUserRule :
        ConversationActionRuleBase<InviteUserToChatAction>
    {
        public override void Validate(Chat chat, InviteUserToChatAction action)
        {
            if(!chat.UsersWithData.TryGetValue(action.UserId, out ChatUserData? chatUserData))
            {
                throw new UserDoesNotBelongToChatException(chat.Id, action.UserId);
            }

            if(chatUserData.UserSettings.Role < action.RoleOnJoin)
            {
                throw new ChatUserInvitationForbiddenDueToRoleHierarchyException(chat.Id, action.UserId, action.RoleOnJoin);
            }

            if(!chatUserData.UserSettings.CanAddUserByLink)
            {
                throw new ChatUserAdditionDisabledByUserSettingsException(chat.Id, action.UserId);
            }
        }
    }
}
