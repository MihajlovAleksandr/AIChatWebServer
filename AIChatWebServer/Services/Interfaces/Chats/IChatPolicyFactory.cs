using AIChatWebServer.Models.Chats;

namespace AIChatWebServer.Services.Interfaces.Chats
{
    public interface IChatPolicyFactory
    {
        IChatRulesValidator Create(ChatType chatType);
    }
}
