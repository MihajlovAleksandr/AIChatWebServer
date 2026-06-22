using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.UserPermissions
{
    public class EditMessageSettingsUserRule : ConversationActionRuleBase<EditMessageAction>
    {
        public override void Validate(Chat chat, EditMessageAction action)
        {
            if (!chat.UsersWithData.TryGetValue(action.UserId, out ChatUserData? chatUserData))
            {
                throw new UserDoesNotBelongToChatException(chat.Id, action.UserId);
            }

            if(!chatUserData.UserSettings.Messages.EditMessagesEnabled)
                throw new MessageModificationDisabledByUserSettingsException(chat.Id, action.UserId, action.Message.Id);
        }
    }
}
