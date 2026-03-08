using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Links;
using AIChatWebServer.Services.Interfaces;
using AIChatWebServer.Services.Interfaces.Chats;
using System.Text.Json;

namespace AIChatWebServer.Services.Implementations.Chats
{
    public class ChatLinkService(IChatService chatService, ILinkService linkService,
        IConversationActionValidator conversationActionValidator,
        IConfiguration configuration) : IChatLinkService
    {                            
        private readonly IChatService _chatService = chatService;
        private readonly ILinkService _linkService = linkService;
        private readonly IConversationActionValidator _conversationActionValidator = conversationActionValidator;
        private readonly int ExpireDays = int.Parse(configuration["Links:ChatInvite:ExpireDays"]
                ?? throw new InvalidOperationException("ChatInvite ExpireDays is not configured."));
        private readonly int MaxUsage = int.Parse(configuration["Links:ChatInvite:MaxUsage"]
            ?? throw new InvalidOperationException("ChatInvite MaxUsage is not configured."));

        public async Task<string> CreateInviteLink(Guid chatId, InviteUserToChatAction action, CancellationToken ct = default)
        {
            Chat chat = await _chatService.GetById(chatId, ct);

            _conversationActionValidator.Validate(chat, action);

            string payloadJson =
                JsonSerializer.Serialize(
                    new ChatInvitePayload(chatId, action.RoleOnJoin, action.ChatName));

            return await _linkService.CreateAsync(LinkType.ChatInvite, payloadJson,
                action.UserId, DateTime.UtcNow.AddDays(ExpireDays), MaxUsage ,ct);
        }

        public async Task<Chat> EnterChatViaInviteLink(string token, Guid userId, CancellationToken ct = default)
        {
            Link link = await _linkService.ExecuteAsync(token, ct);

            ChatInvitePayload payload = link.GetPayload<ChatInvitePayload>();

            await _chatService.ExecuteAction(payload.ChatId, new AddUserAction(link.CreatedBy, userId, ChatSearchType.Link, payload.RoleOnJoin, payload.ChatName), ct);

            return await _chatService.GetById(payload.ChatId, ct);
        }
    }
}
