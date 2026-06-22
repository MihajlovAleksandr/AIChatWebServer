using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.Chat;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.ChatPermissions
{
    public class InviteUserSettingsChatRule :
        ConversationActionRuleBase<InviteUserToChatAction>
    {
        public override void Validate(Chat chat, InviteUserToChatAction action)
        {
            if (!chat.Settings.Members.AllowAddByLink)
            {
                throw new ChatUserAdditionDisabledByChatSettingsException(chat.Id);
            }
        }
    }
}
