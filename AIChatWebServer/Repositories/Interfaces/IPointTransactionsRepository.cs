using AIChatWebServer.Contracts.UnitOfWork.Interfaces;
using AIChatWebServer.Models.Ranks;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IPointTransactionsRepository : ITransactionalScope<IPointTransactionsRepository>
    {
        Task<PointTransaction> CreateAsync(Guid userId, int amount, string type, string? reason = null, Guid? referenceId = null, CancellationToken ct = default);
        Task<List<PointTransaction>> GetByUserIdAsync(Guid userId, int limit, int offset, CancellationToken ct = default);
        Task<List<PointTransaction>> GetByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken ct = default);
        Task<PointTransaction?> GetByReferenceIdAsync(Guid referenceId, CancellationToken ct = default);
        Task<int> GetSumByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken ct = default);
        Task<List<PointTransaction>> GetByTypeAsync(Guid userId, string type, CancellationToken ct = default);
        Task<int> GetUserBalanceAtDateAsync(Guid userId, DateTime date, CancellationToken ct = default);
    }
}