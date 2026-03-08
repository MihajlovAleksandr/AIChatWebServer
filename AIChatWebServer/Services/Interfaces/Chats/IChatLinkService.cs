using AIChatWebServer.Models.Chats;

namespace AIChatWebServer.Services.Interfaces.Chats
{
    public interface IChatLinkService
    {
        Task<string> CreateInviteLink(Guid chatId, InviteUserToChatAction action, CancellationToken ct = default);
        Task<Chat> EnterChatViaInviteLink(string token, Guid userId, CancellationToken ct = default);
    }
}
