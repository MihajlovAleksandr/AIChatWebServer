using AIChatWebServer.Models.Chats;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Implementations.Chats.Matchmaking.Strategies;
using AIChatWebServer.Services.Interfaces.Chats;
using AIChatWebServer.Services.Interfaces.Chats.Matchmaking;

namespace AIChatWebServer.Services.Implementations.Chats.Matchmaking
{
    public class ChatMatchStrategiesHandlerFactory(
        IUserMatchPredicateFactory userMatchPredicateFactory,
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserRepository userRepository,
        IChatRepository chatRepository,
        IRandomChatService randomChatService,
        IConfiguration configuration) : IChatMatchStrategiesHandlerFactory
    {
        public IChatMatchStrategiesHandler Create()
        {
            return new ChatMatchStrategiesHandler(new Dictionary<ChatType, IChatMatchStrategy>
                {
                    { ChatType.Human, new HumanChatMatchStrategy(
                        userMatchPredicateFactory,
                        unitOfWorkFactory,
                        userRepository,
                        chatRepository,
                        configuration) },
                    { ChatType.Random, new RandomChatMatchStrategy(
                        randomChatService,
                        unitOfWorkFactory,
                        userRepository,
                        chatRepository,
                        configuration) }
                });
        }
    }
}
