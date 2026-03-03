using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.UserPermissions
{
    public class AddUserSettingsUserRule : ConversationActionRuleBase<AddUserAction>
    {
        public override void Validate(Chat chat, AddUserAction action)
        {
            if(!chat.UsersWithData.TryGetValue(action.UserId, out ChatUserData? chatUserData)){
                throw new UserDoesNotBelongToChatException(chat.Id, action.UserId);
            }

            if (chat.UsersWithData.TryGetValue(action.AddedUserId, out _))
            {
                throw new UserAlreadyInChatException(chat.Id, action.AddedUserId);
            }

            if (!chatUserData.UserSettings.CanAddUsersBySearch
                    && action.SearchType == ChatSearchType.Search
                || !chatUserData.UserSettings.CanAddUserByLink
                    && action.SearchType == ChatSearchType.Link)
            {
                throw new ChatUserAdditionDisabledByUserSettingsException(chat.Id, action.UserId);
            }
        }
    }
}
