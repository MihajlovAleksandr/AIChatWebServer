using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.PolicyExceptions;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.ConversationActionRules
{
    public class TextOnlyPrepareSendMessageRule : ConversationActionRuleBase<PrepareSendMessageAction>
    {
        public override void Validate(Chat chat, PrepareSendMessageAction action)
        {
            if (action.AttachmentTypes.Count > 0)
            {
                throw new MessageAttachmentForbiddenException(chat.Id);
            }
        }
    }
}
