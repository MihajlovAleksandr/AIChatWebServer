using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.UserPermissions
{
    public class StartSearchSettingsUserRule : ConversationActionRuleBase<StartSearchChatAction>
    {
        public override void Validate(Chat chat, StartSearchChatAction action)
        {
            if (!chat.UsersWithData.TryGetValue(action.UserId, out ChatUserData? chatUserData))
            {
                throw new UserDoesNotBelongToChatException(chat.Id, action.UserId);
            }

            if (!chatUserData.UserSettings.CanAddUsersBySearch)
            {
                throw new ChatUserAdditionDisabledByUserSettingsException(chat.Id, action.UserId);
            }
        }
    }
}

