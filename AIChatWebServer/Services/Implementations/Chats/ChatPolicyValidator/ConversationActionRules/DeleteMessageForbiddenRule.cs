using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.PolicyExceptions;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.ConversationActionRules
{
    public class DeleteMessageForbiddenRule : ConversationActionRuleBase<DeleteMessageAction>
    {
        public override void Validate(Chat chat, DeleteMessageAction action)
        {
            throw new MessageDeletionForbiddenByPolicyException(action.Message.Id);
        }
    }
}
