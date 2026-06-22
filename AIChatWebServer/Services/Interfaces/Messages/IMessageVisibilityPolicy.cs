using AIChatWebServer.Models.Chats;

namespace AIChatWebServer.Services.Interfaces.Messages
{
    public interface IMessageVisibilityPolicy
    {
        bool ShouldIncludeStatuses(ChatType chatType);
    }
}
