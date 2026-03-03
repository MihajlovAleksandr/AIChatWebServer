using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.PolicyExceptions;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.ConversationActionRules
{
    public sealed class SettingsModificationForbiddenRule
        : ConversationActionRuleBase<ChangeChatSettingsAction>
    {
        public override void Validate(Chat chat, ChangeChatSettingsAction action)
        {
            throw new ChatSettingsModificationForbiddenException(chat.Id);
        }
    }
}
