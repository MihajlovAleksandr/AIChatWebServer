using AIChatWebServer.Models.Chats;

namespace AIChatWebServer.Services.Interfaces.Chats
{
    public interface IChatRulesValidator
    {
        void Validate(Models.Chats.Chat chat, ConversationAction chatAction);
    }
}
