using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.User;
using AIChatWebServer.Models.User;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.Chats.Matchmaking;

namespace AIChatWebServer.Services.Implementations.Chats.Matchmaking.Strategies
{
    public class HumanChatMatchStrategy(
        IUserMatchPredicateFactory userMatchPredicateFactory,
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserRepository userRepository,
        IChatRepository chatRepository,
        IConfiguration configuration) : IChatMatchStrategy
    {
        private readonly IUserMatchPredicateFactory _userMatchPredicateFactory = userMatchPredicateFactory;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory = unitOfWorkFactory;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IChatRepository _chatRepository = chatRepository;
        private readonly int _expiredTime = int.Parse(configuration["Matchmaking:ExpiredHours"]
            ?? throw new ArgumentException("Matchmaking ExpiredHours is not configured."));

        public ChatType MatchType => ChatType.Human;

        public async Task MatchUserAsync(
            Guid userId,
            string userPredicate,
            string chatName,
            CancellationToken ct)
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
                await uow.Matchmaking.AcquireCandidatesAsync(
                    entry.UserId,
                    entry.ChatType,
                    10,
                    ct);

            Guid? matchedUserId = null;
            string? matchedChatName = null;

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
                    await uow.Matchmaking.CompleteAsync(
                        [entry.Id,
                        candidate.Id],
                        ct);

                    matchedUserId = candidate.UserId;
                    matchedChatName = candidate.ChatName;

                    break;
                }
            }

            await uow.CommitAsync(ct);

            if (matchedUserId != null)
            {
                await _chatRepository.CreateAsync(
                    MatchType,
                    new Dictionary<Guid, string>
                    {
                        { matchedUserId.Value, matchedChatName! },
                        { userId, chatName }
                    }, ct);
            }
        }
    }
}
