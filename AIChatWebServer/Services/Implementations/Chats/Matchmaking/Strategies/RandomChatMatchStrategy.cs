using AIChatWebServer.Contracts.UnitOfWork.Interfaces;
using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Chats.Matchmaking;
using AIChatWebServer.Models.Chats.RandomChat;
using AIChatWebServer.Models.Exceptions.Implementations.User;
using AIChatWebServer.Models.User;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Background;
using AIChatWebServer.Services.Interfaces.Chats.Matchmaking;
using AIChatWebServer.Services.Interfaces.Chats.RandomChatGame;

namespace AIChatWebServer.Services.Implementations.Chats.Matchmaking.Strategies
{
    public class RandomChatMatchStrategy(
        IRandomChatService randomChatService,
        IChatGameService chatGameService,
        IUnitOfWorkFactory unitOfWorkFactory,
        IMatchmakingRepository matchmakingRepository,
        IUserRepository userRepository,
        IChatRepository chatRepository, 
        IConfiguration configuration,
        IBackgroundJobService backgroundJobService) : IChatMatchStrategy
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory = unitOfWorkFactory;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IChatRepository _chatRepository = chatRepository;
        private readonly IMatchmakingRepository _matchmakingRepository = matchmakingRepository;
        private readonly IChatGameService _chatGameService = chatGameService;
        private readonly IRandomChatService _randomChatService = randomChatService;
        private readonly int _expiredTime = int.Parse(configuration["Matchmaking:ExpiredHours"] 
            ?? throw new ArgumentException("Matchmaking ExpiredHours is not configured."));
        private readonly Guid aIId = Guid.Parse(configuration["SystemUsers:AIId"] 
            ?? throw new ArgumentException("AI Id is not configured."));
        private readonly int probabilityAIChat = int.Parse(configuration["RandomChatSettings:ProbabilityAIChat"]
            ?? throw new ArgumentException("RandomChatSettings ProbabilityAIChat is not configured."));

        public ChatType MatchType => ChatType.Random;

        public async Task<ChatMatchmakingResult?> MatchUserAsync(
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
                return await CreateAIChat(user, chatName, ct);
            }
            else
            {
                return await TryMatchUser(user, chatName, ct);
            }
        }

        private async Task<ChatMatchmakingResult> CreateAIChat(User user, string chatName, CancellationToken ct)
        {
            Guid chatId = await _chatRepository.CreateAsync(MatchType,
                new Dictionary<Guid, string>
                {
                    { aIId, $"RChat With {user.Id}" },
                    { user.Id, chatName}
                },ct);
            backgroundJobService.FireAndForget(async (serviceProvider, jobCt) => {
                IUserProfileGenerator userProfileGenerator = serviceProvider.GetRequiredService<IUserProfileGenerator>();
                await userProfileGenerator.GenerateAync(chatId, user, ct);
            }, "CreateUserProfile", error =>{ });
            Chat chat = await _chatRepository.GetById(chatId)
                ?? throw new ArgumentException();
            await _chatGameService.CreateGameAsync(chatId, chat.UsersWithData[user.Id].Id, chat.UsersWithData[aIId].Id, AiRole.RealAi, ct);
            await _randomChatService.Create(aIId, user.Id, chatId, ct);
            return new ChatMatchmakingResult(chatId);
        }


        private async Task<ChatMatchmakingResult?> TryMatchUser(User user, string chatName, CancellationToken ct)
        {
            await using var uow =
                    await _unitOfWorkFactory.CreateAsync(ct);

            Guid entryId =
                await _matchmakingRepository.EnqueueAsync(
                    user.Id,
                    MatchType,
                    "none",
                    chatName,
                    DateTime.UtcNow.AddHours(_expiredTime),
                    ct);

            var entry =
                await _matchmakingRepository.LockEntryAsync(entryId, ct)
                ?? throw new ArgumentException();

            var candidates =
                await _matchmakingRepository.AcquireCandidatesAsync(
                    entry.UserId,
                    entry.ChatType,
                    1,
                    ct);

            var candidate = candidates.FirstOrDefault();

            if (candidate != null)
            {
                await _matchmakingRepository.CompleteAsync(
                            [entry.Id,
                            candidate.Id],
                            ct);

                await uow.CommitAsync(ct);

                return await CreateHumanChat(candidate.UserId, candidate.ChatName, user.Id, chatName, ct);
            }
            else
            {
                await uow.CommitAsync(ct);
                return null;
            }
        }

        private async Task<ChatMatchmakingResult> CreateHumanChat(Guid firstId, string firstChatName, Guid secondId, string secondChatName, CancellationToken ct)
        {
            Guid chatId = await _chatRepository.CreateAsync(MatchType,
                new Dictionary<Guid, string>
                {
                    { firstId, firstChatName},
                    { secondId, secondChatName}
                }, ct);
            Chat chat = await _chatRepository.GetById(chatId, ct)
                ?? throw new ArgumentException();

            await _randomChatService.Create(firstId, secondId, chatId, ct);

            await _chatGameService.CreateGameAsync(chatId, chat.UsersWithData[secondId].Id, chat.UsersWithData[firstId].Id, AiRole.FakeAi, ct);
            return new ChatMatchmakingResult(chatId);
        }
    }
}
