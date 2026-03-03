using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.ConversationActionRules
{
    public class AddUserValidationRule : ConversationActionRuleBase<AddUserAction>
    {
        public override void Validate(Chat chat, AddUserAction action)
        {
            if (chat.EndTime != null)
            {
                throw new ChatAlreadyEndedException(chat.Id);
            }
        }
    }
}
