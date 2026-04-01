using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.PolicyExceptions;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.ConversationActionRules
{
    public class EditMessageForbiddenRule : ConversationActionRuleBase<EditMessageAction>
    {
        public override void Validate(Chat chat, EditMessageAction action)
        {
            throw new MessageCannotBeModifiedException(action.Message.Id, chat.Id);
        }
    }
}