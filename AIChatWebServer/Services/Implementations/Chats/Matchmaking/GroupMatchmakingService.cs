using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Chats.Matchmaking;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.Matchmaking;
using AIChatWebServer.Models.Sync;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.Chats;
using AIChatWebServer.Services.Interfaces.Chats.Matchmaking;

namespace AIChatWebServer.Services.Implementations.Chats.Matchmaking
{
    public class GroupMatchmakingService(
        IChatAddUserStrategy chatAddUserStrategy,
        IMatchmakingRepository matchmakingRepository,
        IGroupChatSearchRepository groupChatSearchRepository,
        IChatService chatService,
        IConversationActionValidator conversationActionValidator) : IGroupMatchmakingService
    {
        private readonly IChatAddUserStrategy _chatAddUserStrategy = chatAddUserStrategy;
        private readonly IMatchmakingRepository _matchmakingRepository = matchmakingRepository;
        private readonly IGroupChatSearchRepository _groupChatSearchRepository = groupChatSearchRepository;
        private readonly IChatService _chatService = chatService;
        private readonly IConversationActionValidator _conversationActionValidator = conversationActionValidator;

        public async Task CancelSearch(Guid userId, CancellationToken ct = default)
        {
            var matchmakingEntity = await _matchmakingRepository.GetGroupByUserAsync(userId, ct);
            if (matchmakingEntity == null)
            {
                var groupSearchEntity = await _groupChatSearchRepository.GetByUserAsync(userId, ct)
                    ?? throw new UserNotSearchingChatException(userId);

                await _groupChatSearchRepository.CancelAsync(groupSearchEntity.Id, ct);
            }
            else
                await _matchmakingRepository.CancelAsync(matchmakingEntity.Id, ct);
        }

        public async Task<bool> IsSearching(Guid userId, CancellationToken ct = default)
        {
            return 
                await _matchmakingRepository.GetGroupByUserAsync(userId, ct) != null 
                || await _groupChatSearchRepository.GetByUserAsync(userId, ct) != null;
        }

        public async Task<ChatMatchmakingResult?> MatchUserAsync(
            Guid userId,
            string userPredicate,
            string chatName,
            CancellationToken ct = default)
        {
            if (await IsSearching(userId, ct))
                throw new UserAlreadySearchingChatException(userId);

            return await _chatAddUserStrategy.MatchUserAsync(userId, userPredicate, chatName, ct);
        }

        public async Task<ChatMatchmakingResult?> MatchChatAsync(Guid chatId, StartSearchChatAction action, CancellationToken ct = default)
        {
            if (await IsSearching(action.UserId, ct))
                throw new UserAlreadySearchingChatException(action.UserId);

            Chat chat = await _chatService.GetById(chatId, ct);

            _conversationActionValidator.Validate(chat, action);

            return await _chatAddUserStrategy.MatchChatAsync(action.UserId, chatId, action.Slots, action.UserPredicate, ct);
        }

        public async Task<SyncGroupMatchmaking> SyncAsync(Guid userId, CancellationToken ct = default)
        {
            MatchmakingEntry? matchmakingEntry = await _matchmakingRepository.GetGroupByUserAsync(userId, ct);
            if(matchmakingEntry != null)
                return new SyncGroupMatchmaking(true, null);
            GroupChatSearchEntry? groupChatSearchEntry = await _groupChatSearchRepository.GetByUserAsync(userId, ct);
            if (groupChatSearchEntry != null)
                return new SyncGroupMatchmaking(true, groupChatSearchEntry.ChatId);
            return new SyncGroupMatchmaking(false, null);
        }
    }
}

