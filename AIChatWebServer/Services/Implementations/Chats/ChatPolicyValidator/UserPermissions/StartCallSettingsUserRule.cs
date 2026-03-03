using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.UserPermissions
{
    public class StartCallSettingsUserRule : ConversationActionRuleBase<CallAction>
    {
        public override void Validate(Chat chat, CallAction action)
        {
            if (!chat.UsersWithData.TryGetValue(action.UserId, out ChatUserData? chatUserData))
            {
                throw new UserDoesNotBelongToChatException(chat.Id, action.UserId);
            }

            if (!chatUserData.UserSettings.CanStartCalls)
            {
                throw new CallsDisabledByUserSettingsException(chat.Id, action.UserId);
            }
        }
    }
}
