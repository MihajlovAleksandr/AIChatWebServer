using AIChatWebServer.Models.Ranks;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.Chats.RandomChatGame;
using Npgsql;

namespace AIChatWebServer.Services.Implementations.Chats.RandomChatGame
{
    public class PointsCalculator : IPointsCalculator
    {
        private readonly NpgsqlConnection? _conn;
        private readonly NpgsqlTransaction? _tx;
        private readonly IUserPointsRepository _userPointsRepository;

        public PointsCalculator(IUserPointsRepository userPointsRepository)
        {
            _userPointsRepository = userPointsRepository;
            _conn = null;
            _tx = null; 
        }

        public PointsCalculator(IUserPointsRepository userPointsRepository, NpgsqlConnection conn, NpgsqlTransaction tx)
        { 
            _userPointsRepository = userPointsRepository;
            _conn = conn;
            _tx = tx;
        }

        public async Task<(int winnerPoints, int loserPoints)> CalculateAsync(Guid winnerId, Guid loserId)
        {
            var userPointsRepository = _conn == null || _tx == null ? _userPointsRepository : _userPointsRepository.WithTransaction(_conn, _tx);

            UserPoints? winnerPoints = await userPointsRepository.GetByUserIdAsync(winnerId);
            UserPoints? loserPoints = await userPointsRepository.GetByUserIdAsync(loserId);

            int difference = winnerPoints?.TotalPoints ?? 0 - loserPoints?.TotalPoints ?? 0;

            double clampedDifference = Math.Max(-400, Math.Min(400, difference));
            double t = clampedDifference / 400.0;

            int winnerGain;
            int loserLoss;

            if (t <= 0)
            {
                double u = 1 + t;
                double uSquared = u * u;

                winnerGain = (int)Math.Round(40 - 10 * uSquared);
                loserLoss = (int)Math.Round(-40 + 10 * uSquared);
            }
            else
            {
                double tSquared = t * t;

                winnerGain = (int)Math.Round(30 - 20 * tSquared);
                loserLoss = (int)Math.Round(-30 + 25 * tSquared);
            }

            winnerGain = Math.Clamp(winnerGain, 10, 40);
            loserLoss = Math.Clamp(loserLoss, -40, -5);

            return (winnerGain, loserLoss);
        }

        public IPointsCalculator WithTransaction(NpgsqlConnection conn, NpgsqlTransaction tx)
        {
            return new PointsCalculator(_userPointsRepository, conn, tx);
        }
    }
}
