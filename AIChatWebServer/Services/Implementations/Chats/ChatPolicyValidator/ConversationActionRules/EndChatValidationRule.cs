using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.ConversationActionRules
{
    public class EndChatValidationRule : ConversationActionRuleBase<EndChatAction>
    {
        public override void Validate(Chat chat, EndChatAction action)
        {
            if (chat.EndTime != null)
            {
                throw new ChatAlreadyEndedException(chat.Id);
            }
        }
    }
}
