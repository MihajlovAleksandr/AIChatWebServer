using AIChatWebServer.Contracts.UnitOfWork.Interfaces;
using AIChatWebServer.Models.Chats.RandomChat;

namespace AIChatWebServer.Services.Interfaces.Chats.RandomChatGame
{
    public interface IChatGameResultProcessor : ITransactionalScope<IChatGameResultProcessor>
    {
        Task ProcessAsync(ChatGameSession session, CancellationToken ct = default);
    }
}
