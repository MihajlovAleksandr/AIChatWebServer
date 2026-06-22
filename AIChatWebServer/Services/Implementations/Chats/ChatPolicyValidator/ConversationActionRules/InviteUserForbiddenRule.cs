using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.PolicyExceptions;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.ConversationActionRules
{
    public class InviteUserForbiddenRule :
        ConversationActionRuleBase<InviteUserToChatAction>
    {
        public override void Validate(Chat chat, InviteUserToChatAction action)
        {
            throw new ChatMembersModificationForbiddenException(chat.Id);
        }
    }
}
