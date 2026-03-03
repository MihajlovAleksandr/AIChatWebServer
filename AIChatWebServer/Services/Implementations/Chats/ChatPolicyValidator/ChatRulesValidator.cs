using AIChatWebServer.Models.Chats;
using AIChatWebServer.Services.Interfaces.Chats;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator
{
    public class ChatRulesValidator(IReadOnlyDictionary<Type, IConversationActionRule> chatRules) : IChatRulesValidator
    {
        private readonly IReadOnlyDictionary<Type, IConversationActionRule> _chatRules = chatRules;

        public void Validate(Chat chat, ConversationAction chatAction)
        {
            if(_chatRules.TryGetValue(chatAction.GetType(), out IConversationActionRule? rule)){
                rule.Validate(chat, chatAction);
            }
        }
    }
}
