using AIChatWebServer.Models.Ranks;
using AIChatWebServer.Models.User;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.Chats.Ranks;
using Npgsql;

namespace AIChatWebServer.Services.Implementations.Chats.Ranks
{
    public sealed class PointsService(
        IRankLevelsRepository rankLevelsRepository,
        IUserPointsRepository userPointsRepository,
        IPointTransactionsRepository pointTransactionsRepository,
        IUserRankHistoryRepository userRankHistoryRepository) : IPointsService
    {
        private readonly IRankLevelsRepository _rankLevelsRepository = rankLevelsRepository;
        private readonly IUserPointsRepository _userPointsRepository = userPointsRepository;
        private readonly IPointTransactionsRepository _pointTransactionsRepository = pointTransactionsRepository;
        private readonly IUserRankHistoryRepository _userRankHistoryRepository = userRankHistoryRepository;

        private NpgsqlConnection? _connection;
        private NpgsqlTransaction? _transaction;

        public IPointsService WithTransaction(NpgsqlConnection conn, NpgsqlTransaction tx)
        {
            return new PointsService(
                _rankLevelsRepository.WithTransaction(conn, tx),
                _userPointsRepository.WithTransaction(conn, tx),
                _pointTransactionsRepository.WithTransaction(conn, tx),
                _userRankHistoryRepository.WithTransaction(conn, tx))
            {
                _connection = conn,
                _transaction = tx
            };
        }

        public async Task TransactPointsAsync(
            Guid userId,
            int amount,
            string type,
            string? reason = null,
            Guid? referenceId = null,
            CancellationToken ct = default)
        {
            if (amount == 0)
                return;

            var rankRepo = _rankLevelsRepository;
            var pointsRepo = _userPointsRepository;
            var transactionRepo = _pointTransactionsRepository;
            var historyRepo = _userRankHistoryRepository;

            _ = await pointsRepo.FindOrCreateByUserIdAsync(userId, ct);
            
            UserPoints userPoints;
            if (amount > 0)
            {
                userPoints = await pointsRepo.IncrementPointsAsync(userId, amount, ct);
            }
            else
            {
                userPoints = await pointsRepo.DecrementPointsAsync(userId, Math.Abs(amount), ct);
            }

            await transactionRepo.CreateAsync(userId, amount, type, reason, referenceId, ct);

            await UpdateRankIfNeededAsync(userId, userPoints.TotalPoints, rankRepo, historyRepo, ct);
        }

        public async Task SetPointsAsync(
            Guid userId,
            int newTotalPoints,
            string reason,
            CancellationToken ct = default)
        {
            var rankRepo = _rankLevelsRepository;
            var pointsRepo = _userPointsRepository;
            var transactionRepo = _pointTransactionsRepository;
            var historyRepo = _userRankHistoryRepository;

            var currentPoints = await pointsRepo.GetByUserIdAsync(userId, ct);
            var oldTotalPoints = currentPoints?.TotalPoints ?? 0;
            var difference = newTotalPoints - oldTotalPoints;

            if (difference == 0)
                return;

            var userPoints = await pointsRepo.UpdateTotalPointsAsync(userId, newTotalPoints, ct);

            await transactionRepo.CreateAsync(
                userId,
                difference,
                "ADMIN_ADJUST",
                reason,
                null,
                ct);

            await UpdateRankIfNeededAsync(userId, userPoints.TotalPoints, rankRepo, historyRepo, ct);
        }

        public async Task BulkTransactPointsAsync(
            List<(Guid UserId, int Amount, string Type, string? Reason, Guid? ReferenceId)> operations,
            CancellationToken ct = default)
        {
            if (operations == null || operations.Count == 0)
                return;

            var rankRepo = _rankLevelsRepository;
            var pointsRepo = _userPointsRepository;
            var transactionRepo = _pointTransactionsRepository;
            var historyRepo = _userRankHistoryRepository;

            foreach (var op in operations)
            {
                if (op.Amount == 0)
                    continue;

                _ = await pointsRepo.FindOrCreateByUserIdAsync(op.UserId, ct);

                UserPoints userPoints;

                if (op.Amount > 0)
                {
                    userPoints = await pointsRepo.IncrementPointsAsync(op.UserId, op.Amount, ct);
                }
                else
                {
                    userPoints = await pointsRepo.DecrementPointsAsync(op.UserId, Math.Abs(op.Amount), ct);
                }

                await transactionRepo.CreateAsync(op.UserId, op.Amount, op.Type, op.Reason, op.ReferenceId, ct);

                await UpdateRankIfNeededAsync(op.UserId, userPoints.TotalPoints, rankRepo, historyRepo, ct);
            }
        }

        public async Task RollbackTransactionAsync(
            Guid transactionId,
            string reason,
            CancellationToken ct = default)
        {
            var rankRepo = _rankLevelsRepository;
            var pointsRepo = _userPointsRepository;
            var transactionRepo = _pointTransactionsRepository;
            var historyRepo = _userRankHistoryRepository;

            var originalTransaction = await transactionRepo.GetByReferenceIdAsync(transactionId, ct);

            if (originalTransaction == null)
                throw new InvalidOperationException($"Transaction {transactionId} not found");

            var rollbackAmount = -originalTransaction.Amount;

            UserPoints userPoints;

            if (rollbackAmount > 0)
            {
                userPoints = await pointsRepo.IncrementPointsAsync(originalTransaction.UserId, rollbackAmount, ct);
            }
            else if (rollbackAmount < 0)
            {
                userPoints = await pointsRepo.DecrementPointsAsync(originalTransaction.UserId, Math.Abs(rollbackAmount), ct);
            }
            else
            {
                return;
            }

            await transactionRepo.CreateAsync(
                originalTransaction.UserId,
                rollbackAmount,
                "ROLLBACK",
                $"Rollback of transaction {transactionId}: {reason}",
                transactionId,
                ct);

            await UpdateRankIfNeededAsync(originalTransaction.UserId, userPoints.TotalPoints, rankRepo, historyRepo, ct);
        }

        public async Task TransferPointsAsync(
            Guid fromUserId,
            Guid toUserId,
            int amount,
            string reason,
            CancellationToken ct = default)
        {
            if (amount <= 0)
                throw new ArgumentException("Transfer amount must be positive", nameof(amount));

            if (fromUserId == toUserId)
                throw new ArgumentException("Cannot transfer points to the same user");

            var rankRepo = _rankLevelsRepository;
            var pointsRepo = _userPointsRepository;
            var transactionRepo = _pointTransactionsRepository;
            var historyRepo = _userRankHistoryRepository;

            var transferId = Guid.NewGuid();

            _ = pointsRepo.FindOrCreateByUserIdAsync(fromUserId, ct);   

            var fromUserPoints = await pointsRepo.DecrementPointsAsync(fromUserId, amount, ct);

            await transactionRepo.CreateAsync(
                fromUserId,
                -amount,
                "TRANSFER_OUT",
                $"Transfer to {toUserId}: {reason}",
                transferId,
                ct);

            await UpdateRankIfNeededAsync(fromUserId, fromUserPoints.TotalPoints, rankRepo, historyRepo, ct);

            _ = pointsRepo.FindOrCreateByUserIdAsync(toUserId, ct);
            var toUserPoints = await pointsRepo.IncrementPointsAsync(toUserId, amount, ct);

            await transactionRepo.CreateAsync(
                toUserId,
                amount,
                "TRANSFER_IN",
                $"Transfer from {fromUserId}: {reason}",
                transferId,
                ct);

            await UpdateRankIfNeededAsync(toUserId, toUserPoints.TotalPoints, rankRepo, historyRepo, ct);
        }

        public async Task<List<UserPoints>> GetLeaderboardAsync(int limit, int offset, CancellationToken ct = default)
        {
            return await _userPointsRepository.GetLeaderboardAsync(limit, offset, ct);
        }

        public async Task<int> GetLeaderboardTotalCountAsync(CancellationToken ct = default)
        {
            return await _userPointsRepository.GetLeaderboardTotalCountAsync(ct);
        }

        private async Task UpdateRankIfNeededAsync(
            Guid userId,
            int currentPoints,
            IRankLevelsRepository rankRepo,
            IUserRankHistoryRepository historyRepo,
            CancellationToken ct)
        {
            var newRank = await rankRepo.FindRankByPointsAsync(currentPoints, ct);

            if (newRank == null)
                return;

            var currentRank = await historyRepo.GetCurrentRankByUserIdAsync(userId, ct);

            if (currentRank == null || currentRank.RankId != newRank.Id)
            {
                await historyRepo.CreateAsync(userId, newRank.Id, currentPoints, ct);
            }
        }
    }
}