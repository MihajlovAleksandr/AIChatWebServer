using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Chats.Matchmaking;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.Matchmaking;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.Chats.Matchmaking;

namespace AIChatWebServer.Services.Implementations.Chats.Matchmaking
{
    public class DirectMatchmakingService(
        IChatMatchStrategiesHandlerFactory chatMatchStrategiesHandlerFactory,
        IMatchmakingRepository matchmakingRepository) : IDirectMatchmakingService
    {
        private readonly IChatMatchStrategiesHandlerFactory _chatMatchStrategiesHandlerFactory = chatMatchStrategiesHandlerFactory;
        private readonly IMatchmakingRepository _matchmakingRepository = matchmakingRepository;

        public async Task CancelSearch(Guid userId, CancellationToken ct = default)
        {
            var matchmakingEntity = await _matchmakingRepository.GetChatByUserAsync(userId, ct) 
                ?? throw new UserNotSearchingChatException(userId);
            await _matchmakingRepository.CancelAsync(matchmakingEntity.Id, ct);
        }

        public async Task<bool> IsSearching(Guid userId, CancellationToken ct = default)
        {
            var matchmakingEntity = await _matchmakingRepository.GetChatByUserAsync(userId, ct);
            return matchmakingEntity != null;
        }

        public async Task<ChatMatchmakingResult?> MatchUserAsync(
            ChatType chatType,
            Guid userId,
            string userPredicate,
            string chatName,
            CancellationToken ct = default)
        {
            if (await IsSearching(userId, ct))
                throw new UserAlreadySearchingChatException(userId);

            return await _chatMatchStrategiesHandlerFactory.Create().MatchUserAsync(
                chatType,
                userId,
                userPredicate,
                chatName,
                ct);
        }
    }
}