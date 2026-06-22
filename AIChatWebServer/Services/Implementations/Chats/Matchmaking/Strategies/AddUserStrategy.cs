using AIChatWebServer.Contracts.UnitOfWork.Interfaces;
using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Chats.Matchmaking;
using AIChatWebServer.Models.Exceptions.Implementations.User;
using AIChatWebServer.Models.User;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.Chats;
using AIChatWebServer.Services.Interfaces.Chats.Matchmaking;

namespace AIChatWebServer.Services.Implementations.Chats.Matchmaking.Strategies
{
    public class AddUserStrategy(IUserMatchPredicateFactory userMatchPredicateFactory,
        IUnitOfWorkFactory unitOfWorkFactory,
        IMatchmakingRepository m1athchmakingRepository,
        IGroupChatSearchRepository groupRepository, 
        IUserRepository userRepository,
        IChatService chatService,
        IConfiguration configuration) : IChatAddUserStrategy
    {
        private readonly IUserMatchPredicateFactory _userMatchPredicateFactory = userMatchPredicateFactory;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory = unitOfWorkFactory;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMatchmakingRepository _mathchmakingRepository = m1athchmakingRepository;
        private readonly IGroupChatSearchRepository _groupRepository = groupRepository;
        private readonly IChatService _chatService = chatService;
        private readonly int _expiredTime = int.Parse(configuration["Matchmaking:ExpiredHours"]
            ?? throw new ArgumentException("Matchmaking ExpiredHours is not configured."));

        public ChatType MatchType => ChatType.Group;

        public async Task<ChatMatchmakingResult?> MatchUserAsync(Guid userId, string userPredicate, string chatName, CancellationToken ct)
        {
            IUserMatchPredicate predicate =
                _userMatchPredicateFactory.Create(userPredicate);

            User user =
                await _userRepository.GetByIdAsync(userId, ct)
                ?? throw new UserNotFoundException(userId);

            await using var uow =
                await _unitOfWorkFactory.CreateAsync(ct);

            IMatchmakingRepository matchmakingRepository = uow.WithTransaction(_mathchmakingRepository);
            IGroupChatSearchRepository groupRepository = uow.WithTransaction(_groupRepository);

            Guid entryId =
                await _mathchmakingRepository.EnqueueAsync(
                    userId,
                    MatchType,
                    userPredicate,
                    chatName,
                    DateTime.UtcNow.AddHours(_expiredTime),
                    ct);

            var entry =
                await matchmakingRepository.LockEntryAsync(entryId, ct)
                ?? throw new ArgumentException();

            var candidates =
                await groupRepository.AcquireCandidatesAsync(
                    entry.UserId,
                    10,
                    ct);

            (Guid chatId, Guid userId)? chatMatch = null;

            foreach (var candidate in candidates)
            {
                User? candidateUser =
                    await _userRepository.GetByIdAsync(candidate.UserId, ct);

                if (candidateUser == null) continue;


                IUserMatchPredicate candidatePredicate =
                    _userMatchPredicateFactory.Create(candidate.MatchPredicate);

                if (candidatePredicate.TryMatch(candidateUser, user) &&
                    predicate.TryMatch(user, candidateUser))
                {
                    Chat chat = await _chatService.GetById(candidate.ChatId, ct);

                    if (chat.UsersWithData.TryGetValue(userId, out _))
                        continue;

                    chatMatch = new(candidate.ChatId, candidate.UserId);
                    break;
                }
            }

            await uow.CommitAsync(ct);

            if (chatMatch != null)
            {
                await _chatService.ExecuteAction(chatMatch.Value.chatId, 
                    new AddUserAction(chatMatch.Value.userId, userId,
                    ChatSearchType.Search, ChatUserRole.Member, chatName), ct);

                return new GroupMatchmakingResult(chatMatch.Value.chatId, chatMatch.Value.userId);
            }
            return null;
        }

        public async Task<ChatMatchmakingResult?> MatchChatAsync(Guid userId, Guid chatId, int slot, string userPredicate, CancellationToken ct)
        {
            IUserMatchPredicate predicate =
                _userMatchPredicateFactory.Create(userPredicate);

            User user =
                await _userRepository.GetByIdAsync(userId, ct)
                ?? throw new UserNotFoundException(userId);

            if (!user.IsPremium() && slot > 1)
                throw new PremiumRequiredException(user.Id, PremiumFeature.MatchmakingMultipleSlots);

            await using var uow =
                await _unitOfWorkFactory.CreateAsync(ct);

            IGroupChatSearchRepository groupRepository = uow.WithTransaction(_groupRepository);
            IMatchmakingRepository matchmakingRepository = uow.WithTransaction(_mathchmakingRepository);

            Guid entryId =
                await groupRepository.EnqueueAsync(
                    chatId,
                    userId,
                    userPredicate,
                    slot,
                    ct);
            Chat chat = await _chatService.GetById(chatId, ct);
            var entry =
                await groupRepository.LockEntryAsync(entryId, ct)
                ?? throw new ArgumentException();

            var candidates =
                await matchmakingRepository.AcquireCandidatesAsync(
                    entry.UserId,
                    MatchType,
                    10,
                    ct);

            (Guid userId, string chatName)? userMatch = null;
            foreach (var candidate in candidates)
            {
                if (chat.UsersWithData.TryGetValue(candidate.UserId, out _))
                    continue;

                User? candidateUser =
                    await _userRepository.GetByIdAsync(candidate.UserId, ct);

                if (candidateUser == null) continue;

                IUserMatchPredicate candidatePredicate =
                    _userMatchPredicateFactory.Create(candidate.MatchPredicate);

                if (candidatePredicate.TryMatch(candidateUser, user) &&
                    predicate.TryMatch(user, candidateUser))
                {
                    await matchmakingRepository.CompleteAsync([candidate.Id], ct);
                    await groupRepository.CompleteAsync([entry.Id], ct);

                    userMatch = new(candidate.UserId, candidate.ChatName);
                    break;
                }
            }

            await uow.CommitAsync(ct);

            if (userMatch != null)
            {
                await _chatService.ExecuteAction(chatId,
                    new AddUserAction(userId, userMatch.Value.userId, ChatSearchType.Search, ChatUserRole.Member, userMatch.Value.chatName), ct);

                return new GroupMatchmakingResult(chatId, userId);
            }

            return null;
        }
    }
}
