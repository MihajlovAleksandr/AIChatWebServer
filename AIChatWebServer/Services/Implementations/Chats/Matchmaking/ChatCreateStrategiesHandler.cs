using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat;
using AIChatWebServer.Services.Interfaces.Chats.Matchmaking;

namespace AIChatWebServer.Services.Implementations.Chats.Matchmaking
{
    public class ChatCreateStrategiesHandler(IReadOnlyDictionary<ChatType, IChatCreateStrategy> chatCreateStrategies) : IChatCreateStrategiesHandler
    {
        private readonly IReadOnlyDictionary<ChatType, IChatCreateStrategy> _chatCreateStrategies = chatCreateStrategies;
        public async Task<Guid> CreateAsync(ChatType chatType, Guid userId, string chatName, CancellationToken ct)
        {
            if (_chatCreateStrategies.TryGetValue(chatType, out IChatCreateStrategy? chatCreateStrategy))
            {
                return await chatCreateStrategy.CreateAsync(userId, chatName, ct);
            }
            else
            {
                throw new ChatTypeNotSupportedException(chatType);
            }
        }
    }
}
