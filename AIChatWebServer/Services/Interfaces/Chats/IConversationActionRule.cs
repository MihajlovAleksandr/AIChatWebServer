using AIChatWebServer.Models.Chats;

namespace AIChatWebServer.Services.Interfaces.Chats
{
    public interface IConversationActionRule<T> : IConversationActionRule
        where T : ConversationAction
    {
        void Validate(Chat chat, T action);
    }

    public interface IConversationActionRule
    {
        bool Supports(ConversationAction action);
        void Validate(Chat chat, ConversationAction action);
    }
}
