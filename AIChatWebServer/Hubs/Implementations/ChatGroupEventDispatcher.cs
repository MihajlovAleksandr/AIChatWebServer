using AIChatWebServer.DTO.Response;
using AIChatWebServer.Hubs.Interfaces;

namespace AIChatWebServer.Hubs.Implementations
{
    public class ChatGroupEventDispatcher(
        IMessageDispatcher messageDispatcher): IChatGroupEventDispatcher
    {
        private readonly IMessageDispatcher _messageDispatcher = messageDispatcher;

        public async Task GroupCreated(Guid userId, ChatResponse response)
        {
            await _messageDispatcher.SendToUserAsync(userId, [], nameof(GroupCreated), response, CancellationToken.None);
        }
    }
}
