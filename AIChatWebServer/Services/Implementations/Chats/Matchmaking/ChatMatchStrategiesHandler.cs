using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Chats.Matchmaking;
using AIChatWebServer.Models.Exceptions.Implementations.Chat;
using AIChatWebServer.Services.Interfaces.Chats.Matchmaking;

namespace AIChatWebServer.Services.Implementations.Chats.Matchmaking
{
    public class ChatMatchStrategiesHandler(IReadOnlyDictionary<ChatType, IChatMatchStrategy> chatMatchStrategies) : IChatMatchStrategiesHandler
    {
        private readonly IReadOnlyDictionary<ChatType, IChatMatchStrategy> _chatMatchStrategies = chatMatchStrategies;

        public async Task<ChatMatchmakingResult?> MatchUserAsync(ChatType chatType, Guid userId, string userPredicate, string chatName, CancellationToken ct)
        {
            if(_chatMatchStrategies.TryGetValue(chatType, out IChatMatchStrategy? chatMatchStrategy)){
               return await chatMatchStrategy.MatchUserAsync(userId, userPredicate, chatName, ct);
            }
            else
            {
                throw new ChatTypeNotSupportedException(chatType);
            }
        }
    }
}
