using AIChatWebServer.Contracts.UnitOfWork.Interfaces;
using AIChatWebServer.Models.Ranks;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IUserRankHistoryRepository : ITransactionalScope<IUserRankHistoryRepository>
    {
        Task<UserRankHistory> CreateAsync(Guid userId, int rankId, int pointsAtMoment, CancellationToken ct = default);
        Task<UserRankWithDetails?> GetCurrentRankByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<List<UserRankWithDetails>> GetHistoryByUserIdAsync(Guid userId, int limit, int offset, CancellationToken ct = default);
        Task<List<Guid>> GetUsersWhoChangedRankBetweenDatesAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default);
        Task<UserRankHistory?> GetLastRankChangeByUserAsync(Guid userId, CancellationToken ct = default);
        Task<List<UserRankWithDetails>> GetRankHistoryByDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken ct = default);
        Task<List<Guid>> GetUsersInRankAsync(int rankId, CancellationToken ct = default);
    }
}