using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.User;
using AIChatWebServer.Models.User;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.Chats;
using AIChatWebServer.Services.Interfaces.Chats.Matchmaking;

namespace AIChatWebServer.Services.Implementations.Chats.Matchmaking.Strategies
{
    public class RandomChatMatchStrategy(
        IRandomChatService randomChatService,
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserRepository userRepository,
        IChatRepository chatRepository, 
        IConfiguration configuration) : IChatMatchStrategy
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory = unitOfWorkFactory;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IChatRepository _chatRepository = chatRepository;
        private readonly IRandomChatService _randomChatService = randomChatService;
        private readonly int _expiredTime = int.Parse(configuration["Matchmaking:ExpiredHours"] 
            ?? throw new ArgumentException("Matchmaking ExpiredHours is not configured."));
        private readonly Guid aIId = Guid.Parse(configuration["SystemUsers:AIId"] 
            ?? throw new ArgumentException("AI Id is not configured."));
        private readonly int probabilityAIChat = int.Parse(configuration["RandomChatSettings:ProbabilityAIChat"]
            ?? throw new ArgumentException("RandomChatSettings ProbabilityAIChat is not configured."));

        public ChatType MatchType => ChatType.Random;

        public async Task MatchUserAsync(
            Guid userId,
            string userPredicate,
            string chatName,
            CancellationToken ct)
        {
            User user =
                await _userRepository.GetByIdAsync(userId, ct)
                ?? throw new UserNotFoundException(userId);

            bool isChatAI = Random.Shared.Next(0, 100) < probabilityAIChat;

            if (isChatAI)
            {
                await CreateAIChat(user.Id, chatName, ct);
            }
            else
            {
                await TryMatchUser(user, chatName, ct);
            }
        }

        private async Task CreateAIChat(Guid userId, string chatName, CancellationToken ct)
        {
            Guid chatId = await _chatRepository.CreateAsync(MatchType,
                new Dictionary<Guid, string>
                {
                    { aIId, $"RChat With {userId}" },
                    { userId, chatName}
                },ct);

            await _randomChatService.Create(aIId, userId, chatId, ct);
        }


        private async Task TryMatchUser(User user, string chatName, CancellationToken ct)
        {
            await using var uow =
                    await _unitOfWorkFactory.CreateAsync(ct);

            Guid entryId =
                await uow.Matchmaking.EnqueueAsync(
                    user.Id,
                    MatchType,
                    "none",
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
                    1,
                    ct);

            var candidate = candidates.FirstOrDefault();

            if (candidate != null)
            {
                await uow.Matchmaking.CompleteAsync(
                            [entry.Id,
                            candidate.Id],
                            ct);

                await uow.CommitAsync(ct);

                await CreateHumanChat(candidate.Id, candidate.ChatName, user.Id, chatName, ct);
            }
            else
                await uow.CommitAsync(ct);
        }

        private async Task CreateHumanChat(Guid firstId, string firstChatName, Guid secondId, string secondChatName, CancellationToken ct)
        {
            Guid chatId = await _chatRepository.CreateAsync(MatchType,
                new Dictionary<Guid, string>
                {
                    { firstId, firstChatName},
                    { secondId, secondChatName}
                }, ct);

            await _randomChatService.Create(firstId, secondId, chatId, ct);
        }
    }
}