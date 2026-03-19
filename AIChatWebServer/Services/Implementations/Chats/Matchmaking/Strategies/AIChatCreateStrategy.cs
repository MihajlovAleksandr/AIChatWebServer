using AIChatWebServer.Models.Chats;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.Chats.Matchmaking;

namespace AIChatWebServer.Services.Implementations.Chats.Matchmaking.Strategies
{
    public class AIChatCreateStrategy(IChatRepository chatRepository, IConfiguration configuration) : IChatCreateStrategy
    {
        private readonly Guid aIId = Guid.Parse(configuration["SystemUsers:AIId"] ?? throw new ArgumentException("AI Id is not configured."));
        private readonly IChatRepository _chatRepository = chatRepository;

        public async Task<Guid> CreateAsync(Guid userId, string chatName, CancellationToken ct)
        {
            return await _chatRepository.CreateAsync(ChatType.AI,
                new Dictionary<Guid, string>
                {
                    { userId, chatName },
                    { aIId, $"Chat With {userId}" }
                }, ct);
        }
    }
}
