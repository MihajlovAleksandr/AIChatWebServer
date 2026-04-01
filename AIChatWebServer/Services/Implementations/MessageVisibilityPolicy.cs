using AIChatWebServer.Models.Chats;
using AIChatWebServer.Services.Interfaces;

namespace AIChatWebServer.Services.Implementations
{
    public sealed class MessageVisibilityPolicy : IMessageVisibilityPolicy
    {
        public bool ShouldIncludeStatuses(ChatType chatType)
        {
            return chatType switch
            {
                ChatType.Random => false,
                ChatType.Group => true,
                ChatType.Human => true,
                ChatType.AI => false,
                _ => throw new NotSupportedException()
            };
        }
    }
}
