using AIChatWebServer.Contracts.UnitOfWork.Interfaces;

namespace AIChatWebServer.Services.Interfaces.Chats.RandomChatGame
{
    public interface IPointsCalculator : ITransactionalScope<IPointsCalculator>
    {
        Task<(int winnerPoints, int loserPoints)> CalculateAsync(Guid winnerId, Guid loserId);
    }
}
