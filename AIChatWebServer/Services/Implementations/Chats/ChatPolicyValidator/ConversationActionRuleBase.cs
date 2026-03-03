using AIChatWebServer.Models.Chats;
using AIChatWebServer.Services.Interfaces.Chats;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator
{
    public abstract class ConversationActionRuleBase<T>
        : IConversationActionRule<T>
        where T : Models.Chats.ConversationAction
    {
        public bool Supports(Models.Chats.ConversationAction action)
            => action is T;

        public void Validate(Chat chat, Models.Chats.ConversationAction action)
        {
            Validate(chat, (T)action);
        }

        public abstract void Validate(Chat chat, T action);
    }
}
