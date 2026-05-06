using AIChatWebServer.Models.Chats;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Implementations.Chats.Matchmaking.Strategies;
using AIChatWebServer.Services.Implementations.Chats.RandomChatGame;
using AIChatWebServer.Services.Interfaces.Chats.Matchmaking;
using AIChatWebServer.Services.Interfaces.Chats.RandomChatGame;

namespace AIChatWebServer.Services.Implementations.Chats.Matchmaking
{
    public class ChatMatchStrategiesHandlerFactory(
        IUserMatchPredicateFactory userMatchPredicateFactory,
        IUnitOfWorkFactory unitOfWorkFactory,
        IMatchmakingRepository matchmakingRepository,
        IUserRepository userRepository,
        IChatRepository chatRepository,
        IUserProfileGenerator generator,
        IChatGameService gameService,
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
                        matchmakingRepository,
                        userRepository,
                        chatRepository,
                        configuration) },
                    { ChatType.Random, new RandomChatMatchStrategy(
                        randomChatService,
                        gameService,
                        generator,
                        unitOfWorkFactory,
                        matchmakingRepository,
                        userRepository,
                        chatRepository,
                        configuration) }
                });
        }
    }
}
