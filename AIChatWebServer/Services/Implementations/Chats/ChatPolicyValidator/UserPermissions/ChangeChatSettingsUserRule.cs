using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.UserPermissions
{
    public class ChangeChatSettingsUserRule : ConversationActionRuleBase<ChangeChatSettingsAction>
    {
        public override void Validate(Chat chat, ChangeChatSettingsAction action)
        {
            if (!chat.UsersWithData.TryGetValue(action.UserId, out ChatUserData? chatUserData))
            {
                throw new UserDoesNotBelongToChatException(chat.Id, action.UserId);
            }

            if (!chatUserData.UserSettings.CanChangeChatSettings)
            {
                throw new ChatSettingsChangeDisabledByUserSettingsException(chat.Id, action.UserId);
            }
        }
    }
}
