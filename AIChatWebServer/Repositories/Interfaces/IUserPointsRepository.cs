using AIChatWebServer.Contracts.UnitOfWork.Interfaces;
using AIChatWebServer.Models.Ranks;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IUserPointsRepository : ITransactionalScope<IUserPointsRepository>
    {
        Task<UserPoints> FindOrCreateByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<UserPoints?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<UserPoints> UpdateTotalPointsAsync(Guid userId, int totalPoints, CancellationToken ct = default);
        Task<UserPoints> IncrementPointsAsync(Guid userId, int amount, CancellationToken ct = default);
        Task<UserPoints> DecrementPointsAsync(Guid userId, int amount, CancellationToken ct = default);
        Task<List<UserPoints>> GetByPointsRangeAsync(int minPoints, int maxPoints, CancellationToken ct = default);
        Task<List<UserPoints>> GetLeaderboardAsync(int limit, int offset, CancellationToken ct = default);
        Task<int> GetLeaderboardTotalCountAsync(CancellationToken ct = default);
    }
}