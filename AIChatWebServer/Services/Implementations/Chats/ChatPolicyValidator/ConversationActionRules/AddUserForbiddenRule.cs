using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.PolicyExceptions;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.ConversationActionRules
{
    public sealed class AddUserForbiddenRule :
        ConversationActionRuleBase<AddUserAction>
    {
        public override void Validate(Chat chat, AddUserAction action)
        {
            throw new ChatMembersModificationForbiddenException(chat.Id);
        }
    }
}
