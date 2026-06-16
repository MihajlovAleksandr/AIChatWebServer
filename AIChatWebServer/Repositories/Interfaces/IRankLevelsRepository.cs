using AIChatWebServer.Contracts.UnitOfWork.Interfaces;
using AIChatWebServer.Models.Ranks;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IRankLevelsRepository : ITransactionalScope<IRankLevelsRepository>
    {
        Task<RankLevel?> FindRankByPointsAsync(int points, CancellationToken ct = default);
        Task<RankLevel?> FindRankByPriorityAsync(int priority, CancellationToken ct = default);
        Task<List<RankLevel>> FindAllRanksSortedByPriorityAsync(CancellationToken ct = default);
        Task<RankLevel?> FindRankByMinPointsAsync(int minPoints, CancellationToken ct = default);
        Task<RankLevel?> GetNextRankAsync(int currentRankId, CancellationToken ct = default);
        Task<RankLevel?> GetPreviousRankAsync(int currentRankId, CancellationToken ct = default);
        Task<(RankLevel Current, RankLevel? Next)> GetRankWithNextByPointsAsync(int points, CancellationToken ct = default);
    }
}