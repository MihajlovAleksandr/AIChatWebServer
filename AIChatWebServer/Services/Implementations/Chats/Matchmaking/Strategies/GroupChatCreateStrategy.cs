using AIChatWebServer.Models.Chats;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.Chats.Matchmaking;

namespace AIChatWebServer.Services.Implementations.Chats.Matchmaking.Strategies
{
    public class GroupChatCreateStrategy(IChatRepository chatRepository) : IChatCreateStrategy
    {
        private readonly IChatRepository _chatRepository = chatRepository;

        public async Task<Guid> CreateAsync(Guid userId, string chatName, CancellationToken ct)
        {
            return await _chatRepository.CreateAsync(ChatType.Group,
                new Dictionary<Guid, string>
                {
                    { userId, chatName }
                }, ct);
        }
    }
}
