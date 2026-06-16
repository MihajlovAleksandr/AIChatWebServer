using AIChatWebServer.Models.Chats.RandomChat;
using AIChatWebServer.Services.Interfaces.Chats;
using AIChatWebServer.Services.Interfaces.Chats.RandomChatGame;
using AIChatWebServer.Services.Interfaces.Chats.Ranks;
using Npgsql;

namespace AIChatWebServer.Services.Implementations.Chats.RandomChatGame
{
    public class ChatGameResultProcessor : IChatGameResultProcessor
    {
        private readonly IChatService _chatService;
        private readonly IPointsService _pointsService;
        private readonly IPointsCalculator _pointsCalculator;
        private readonly NpgsqlConnection? _conn;
        private readonly NpgsqlTransaction? _tx;

        public ChatGameResultProcessor(IChatService chatService, IPointsService pointsService, IPointsCalculator pointsCalculator)
        {
            _chatService = chatService;
            _pointsCalculator = pointsCalculator;
            _pointsService = pointsService;
            _conn = null;
            _tx = null;
        }

        public ChatGameResultProcessor(IChatService chatService, IPointsService pointsService, IPointsCalculator pointsCalculator, NpgsqlConnection conn, NpgsqlTransaction tx)
        {
            _chatService = chatService;
            _pointsCalculator = pointsCalculator;
            _pointsService = pointsService;
            _conn = conn;
            _tx = tx;
        }

        public IChatGameResultProcessor WithTransaction(NpgsqlConnection conn, NpgsqlTransaction tx)
        {
            return new ChatGameResultProcessor(_chatService, _pointsService, _pointsCalculator, conn, tx);
        }

        public async Task ProcessAsync(ChatGameSession session, CancellationToken ct = default)
        {
            Guid winnerId = await _chatService.GetUserIdByChatUserId(session.GetWinner()?.UserChatId ?? throw new ArgumentException("winner was null"), ct);
            Guid loserId = await _chatService.GetUserIdByChatUserId(session.GetLoser()?.UserChatId ?? throw new ArgumentException("loser was null"), ct);

            IPointsService pointsService = _conn == null || _tx == null ? _pointsService : _pointsService.WithTransaction(_conn, _tx);
            IPointsCalculator pointsCalculator = _conn == null || _tx == null ? _pointsCalculator : _pointsCalculator.WithTransaction(_conn, _tx);

            var (winnerPoints, loserPoints) = await pointsCalculator.CalculateAsync(winnerId, loserId);
            
            await pointsService.TransactPointsAsync(winnerId, winnerPoints, "AIChat", "Win Round", session.Id, ct);
            await pointsService.TransactPointsAsync(loserId, loserPoints, "AIChat", "Loose Round", session.Id, ct);
        }
    }
}
