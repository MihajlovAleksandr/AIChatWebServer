using AIChatWebServer.Models.Chats;

namespace AIChatWebServer.Services.Interfaces.Chats
{
    public interface IConversationActionValidator
    {
        void Validate(Chat chat, ConversationAction chatAction);
    }
}
