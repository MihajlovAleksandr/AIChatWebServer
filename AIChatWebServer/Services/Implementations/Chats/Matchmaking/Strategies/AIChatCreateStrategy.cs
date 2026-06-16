using AIChatWebServer.Models.AI;
using AIChatWebServer.Models.Chats;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.AI;
using AIChatWebServer.Services.Interfaces.Chats.Matchmaking;

namespace AIChatWebServer.Services.Implementations.Chats.Matchmaking.Strategies
{
    public class AIChatCreateStrategy(IChatRepository chatRepository, IAISettingsService aISettingsService, IConfiguration configuration) : IChatCreateStrategy
    {
        private readonly Guid aIId = Guid.Parse(configuration["SystemUsers:AIId"] ?? throw new ArgumentException("AI Id is not configured."));
        private readonly IChatRepository _chatRepository = chatRepository;
        private readonly IAISettingsService _aISettingsService = aISettingsService;

        public async Task<Guid> CreateAsync(Guid userId, string chatName, CancellationToken ct)
        {
            Guid id = await _chatRepository.CreateAsync(ChatType.AI,
                new Dictionary<Guid, string>
                {
                    { userId, chatName },
                    { aIId, $"Chat With {userId}" }
                }, ct);

            await _aISettingsService.CreateOrUpdate(userId, id, AIModel.Default, null, ct);
            return id;
        }
    }
}
