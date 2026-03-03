using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.ConversationActionRules
{
    public class UpdateChatNameValidationRule : ConversationActionRuleBase<UpdateNameAction>
    {
        public override void Validate(Chat chat, UpdateNameAction action)
        {
            if (!chat.UsersWithData.ContainsKey(action.UserId))
            {
                throw new UserNotInChatException(chat.Id, action.UserId);
            }
        }
    }
}
