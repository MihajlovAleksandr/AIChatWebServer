using AIChatWebServer.Models.Sync;
using AIChatWebServer.Services.Interfaces.Chats;
using AIChatWebServer.Services.Interfaces.Chats.Matchmaking;
using AIChatWebServer.Services.Interfaces.Messages;
using AIChatWebServer.Services.Interfaces.Utils;

namespace AIChatWebServer.Services.Implementations.Utils
{
    public sealed class SyncService(
        IChatService chatService,
        IMessageService messageService,
        IDirectMatchmakingService directMatchmakingService,
        IGroupMatchmakingService groupMatchmakingService
        ) : ISyncService
    {
        private readonly IChatService _chatService = chatService;
        private readonly IMessageService _messageService = messageService;
        private readonly IDirectMatchmakingService _directMatchmakingService = directMatchmakingService;
        private readonly IGroupMatchmakingService _groupMatchmakingService = groupMatchmakingService;

        public async Task<SyncModel> SyncAsync(Guid userId, DateTime? lastOnline, CancellationToken ct = default)
        {
            DateTime since = lastOnline ?? DateTime.MinValue;
            return new SyncModel(
                new SyncMatchmaking(
                    new SyncChatMatchmaking(
                        await _directMatchmakingService.IsSearching(userId, ct)),
                    await _groupMatchmakingService.SyncAsync(userId, ct)
                ), 
                await _messageService.SyncAsync(userId, since, ct),
                await _chatService.SyncAsync(userId, since, ct));
        }
    }
}
