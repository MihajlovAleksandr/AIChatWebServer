using AIChatWebServer.Models.Chats;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Implementations.Chats.Matchmaking;
using AIChatWebServer.Services.Implementations.Chats.Matchmaking.Strategies;

namespace AIChatWebServer.Services.Interfaces.Chats.Matchmaking
{
    public class ChatCreateStrategiesHandlerFactory(IChatRepository chatRepository, IConfiguration configuration) : IChatCreateStrategiesHandlerFactory
    {
        public IChatCreateStrategiesHandler Create()
        {
            return new ChatCreateStrategiesHandler(new Dictionary<ChatType, IChatCreateStrategy>
            {
                { ChatType.AI, new AIChatCreateStrategy(chatRepository, configuration) },
                { ChatType.Group, new GroupChatCreateStrategy(chatRepository) }
            });
        }
    }
}
