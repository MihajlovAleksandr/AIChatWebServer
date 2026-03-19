using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.User;
using AIChatWebServer.Models.User;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.Chats;
using AIChatWebServer.Services.Interfaces.Chats.Matchmaking;

namespace AIChatWebServer.Services.Implementations.Chats.Matchmaking.Strategies
{
    public class AddUserStrategy(IUserMatchPredicateFactory userMatchPredicateFactory,
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserRepository userRepository,
        IChatService chatService,
        IConfiguration configuration) : IChatAddUserStrategy
    {
        private readonly IUserMatchPredicateFactory _userMatchPredicateFactory = userMatchPredicateFactory;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory = unitOfWorkFactory;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IChatService _chatService = chatService;
        private readonly int _expiredTime = int.Parse(configuration["Matchmaking:ExpiredHours"]
            ?? throw new ArgumentException("Matchmaking ExpiredHours is not configured."));

        public ChatType MatchType => ChatType.Group;

        public async Task MatchUserAsync(Guid userId, string userPredicate, string chatName, CancellationToken ct)
        {
            IUserMatchPredicate predicate =
                _userMatchPredicateFactory.Create(userPredicate);

            User user =
                await _userRepository.GetByIdAsync(userId, ct)
                ?? throw new UserNotFoundException(userId);

            await using var uow =
                await _unitOfWorkFactory.CreateAsync(ct);

            Guid entryId =
                await uow.Matchmaking.EnqueueAsync(
                    userId,
                    MatchType,
                    userPredicate,
                    chatName,
                    DateTime.UtcNow.AddHours(_expiredTime),
                    ct);

            var entry =
                await uow.Matchmaking.LockEntryAsync(entryId, ct)
                ?? throw new ArgumentException();

            var candidates =
                await uow.GroupChatSearch.AcquireCandidatesAsync(
                    entry.UserId,
                    10,
                    ct);

            (Guid chatId, Guid UserId)? chatMatch = null;

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
                    await uow.Matchmaking.CompleteAsync([entry.Id], ct);
                    await uow.GroupChatSearch.CompleteAsync([candidate.Id], ct);

                    chatMatch = new(candidate.ChatId, candidate.UserId);
                    break;
                }
            }

            await uow.CommitAsync(ct);

            if (chatMatch != null)
            {
                await _chatService.ExecuteAction(chatMatch.Value.chatId, 
                    new AddUserAction(chatMatch.Value.UserId, userId,
                    ChatSearchType.Search, ChatUserRole.Member, chatName), ct);
            }
        }

        public async Task MatchChatAsync(Guid userId, Guid chatId, int slot, string userPredicate, CancellationToken ct)
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

            Guid entryId =
                await uow.GroupChatSearch.EnqueueAsync(
                    chatId,
                    userId,
                    userPredicate,
                    slot,
                    ct);

            var entry =
                await uow.GroupChatSearch.LockEntryAsync(entryId, ct)
                ?? throw new ArgumentException();

            var candidates =
                await uow.Matchmaking.AcquireCandidatesAsync(
                    entry.UserId,
                    MatchType,
                    10,
                    ct);

            (Guid userId, string chatName)? userMatch = null;
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
                    await uow.Matchmaking.CompleteAsync([candidate.Id], ct);
                    await uow.GroupChatSearch.CompleteAsync([entry.Id], ct);

                    userMatch = new(candidate.UserId, candidate.ChatName);
                    break;
                }
            }

            await uow.CommitAsync(ct);

            if (userMatch != null)
            {
                await _chatService.ExecuteAction(chatId,
                    new AddUserAction(userId, userMatch.Value.userId, ChatSearchType.Search, ChatUserRole.Member, userMatch.Value.chatName), ct);
            }
        }
    }
}
