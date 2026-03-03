using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.PolicyExceptions;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.ConversationActionRules
{
    public class EndChatForbiddenRule : ConversationActionRuleBase<EndChatAction>
    {
        public override void Validate(Chat chat, EndChatAction action)
        {
            throw new ChatCannotBeEndedException(chat.Id);
        }
    }
}
