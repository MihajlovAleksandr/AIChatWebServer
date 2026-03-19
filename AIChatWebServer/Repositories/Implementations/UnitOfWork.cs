using AIChatWebServer.Repositories.Interfaces;
using Npgsql;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class UnitOfWork(
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        IMatchmakingRepository matchmaking,
        IGroupChatSearchRepository groupChatSearch) : IUnitOfWork
    {
        private readonly NpgsqlConnection _conn = conn;
        private readonly NpgsqlTransaction _tx = tx;

        public IMatchmakingRepository Matchmaking { get; } =
                matchmaking.WithTransaction(conn, tx);

        public IGroupChatSearchRepository GroupChatSearch { get; } =
                groupChatSearch.WithTransaction(conn, tx);

        public Task CommitAsync(
            CancellationToken ct = default) =>
            _tx.CommitAsync(ct);

        public Task RollbackAsync(
            CancellationToken ct = default) =>
            _tx.RollbackAsync(ct);

        public async ValueTask DisposeAsync()
        {
            await _tx.DisposeAsync();
            await _conn.DisposeAsync();
        }
    }
}