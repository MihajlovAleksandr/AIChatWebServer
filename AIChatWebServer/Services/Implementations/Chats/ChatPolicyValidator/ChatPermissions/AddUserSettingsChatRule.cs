using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.Chat;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.ChatPermissions
{
    public class AddUserSettingsChatRule : ConversationActionRuleBase<AddUserAction>
    {
        public override void Validate(Chat chat, AddUserAction action)
        {
            if (!chat.Settings.Members.AllowSearchJoin
                    && action.SearchType == ChatSearchType.Search 
                || !chat.Settings.Members.AllowAddByLink 
                    && action.SearchType == ChatSearchType.Link)
            {
                throw new ChatUserAdditionDisabledByChatSettingsException(chat.Id);
            }
        }
    }
}
