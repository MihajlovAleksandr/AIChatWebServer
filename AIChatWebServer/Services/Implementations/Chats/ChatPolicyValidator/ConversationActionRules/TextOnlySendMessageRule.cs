using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.PolicyExceptions;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.ConversationActionRules
{
    public class TextOnlySendMessageRule : ConversationActionRuleBase<SendMessageAction>
    {
        public override void Validate(Chat chat, SendMessageAction action)
        {
            if (action.Attachments.Count > 0)
            {
                throw new MessageAttachmentForbiddenException(chat.Id);
            }
        }
    }
}
