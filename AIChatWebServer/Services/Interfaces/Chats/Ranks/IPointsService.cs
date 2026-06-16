using AIChatWebServer.Contracts.UnitOfWork.Interfaces;
using AIChatWebServer.Models.Ranks;

namespace AIChatWebServer.Services.Interfaces.Chats.Ranks
{
    public interface IPointsService : ITransactionalScope<IPointsService>
    {
        Task TransactPointsAsync(
            Guid userId,
            int amount,
            string type,
            string? reason = null,
            Guid? referenceId = null,
            CancellationToken ct = default);

        Task SetPointsAsync(
            Guid userId,
            int newTotalPoints,
            string reason,
            CancellationToken ct = default);

        Task BulkTransactPointsAsync(
            List<(Guid UserId, int Amount, string Type, string? Reason, Guid? ReferenceId)> operations,
            CancellationToken ct = default);

        Task RollbackTransactionAsync(
            Guid transactionId,
            string reason,
            CancellationToken ct = default);

        Task<List<UserPoints>> GetLeaderboardAsync(
            int limit,
            int offset,
            CancellationToken ct = default);

        Task TransferPointsAsync(
            Guid fromUserId,
            Guid toUserId,
            int amount,
            string reason,
            CancellationToken ct = default);

        Task<int> GetLeaderboardTotalCountAsync(CancellationToken ct = default);
    }
}