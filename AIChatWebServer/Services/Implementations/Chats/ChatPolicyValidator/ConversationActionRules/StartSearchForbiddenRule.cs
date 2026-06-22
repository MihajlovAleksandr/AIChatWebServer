using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.PolicyExceptions;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.ConversationActionRules
{
    public class StartSearchForbiddenRule : ConversationActionRuleBase<StartSearchChatAction>
    {
        public override void Validate(Chat chat, StartSearchChatAction action)
        {
            throw new ChatMembersModificationForbiddenException(chat.Id);
        }
    }
}
