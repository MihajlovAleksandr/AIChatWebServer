using AIChatWebServer.Models.Chats;

namespace AIChatWebServer.Services.Interfaces
{
    public interface IMessageVisibilityPolicy
    {
        bool ShouldIncludeStatuses(ChatType chatType);
    }
}
