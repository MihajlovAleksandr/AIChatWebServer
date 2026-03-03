using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.UserPermissions
{
    public class EndChatUserRule : ConversationActionRuleBase<EndChatAction>
    {
        public override void Validate(Chat chat, EndChatAction action)
        {
            if(!chat.UsersWithData.TryGetValue(action.UserId, out ChatUserData? chatUserData)){
                throw new UserDoesNotBelongToChatException(chat.Id, action.UserId);
            }
            if(chatUserData.UserSettings.Role != ChatUserRole.Owner)
            {
                throw new ChatEndForbiddenForNonOwnerException(chat.Id, action.UserId);
            }
        }
    }
}
