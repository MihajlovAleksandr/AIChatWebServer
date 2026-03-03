using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.PolicyExceptions;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.ConversationActionRules
{
    public class RemoveUserForbiddenRule :
        ConversationActionRuleBase<RemoveUserAction>
    {
        public override void Validate(Chat chat, RemoveUserAction action)
        {
            throw new ChatMembersModificationForbiddenException(chat.Id);
        }
    }
}
