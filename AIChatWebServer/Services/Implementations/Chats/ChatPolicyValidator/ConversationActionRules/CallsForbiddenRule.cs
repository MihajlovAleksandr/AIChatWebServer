using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.PolicyExceptions;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.ConversationActionRules
{
    public sealed class CallsForbiddenRule
        : ConversationActionRuleBase<CallAction>
    {
        public override void Validate(Chat chat, CallAction action)
        {
            throw new ChatCallsForbiddenException(chat.Id);
        }
    }
}
