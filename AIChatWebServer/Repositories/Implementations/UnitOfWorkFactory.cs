using AIChatWebServer.Repositories.Interfaces;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class UnitOfWorkFactory(
        IMatchmakingRepository matchmaking,
        IGroupChatSearchRepository groupChatSearch)
                : BaseRepository, IUnitOfWorkFactory
    {
        private readonly IMatchmakingRepository _matchmaking = matchmaking;
        private readonly IGroupChatSearchRepository _groupChatSearch = groupChatSearch;

        public async Task<IUnitOfWork> CreateAsync(
            CancellationToken ct = default)
        {
            var conn =
                await GetConnectionAsync(ct);

            var tx =
                await conn.BeginTransactionAsync(ct);

            return new UnitOfWork(
                conn,
                tx,
                _matchmaking,
                _groupChatSearch);
        }
    }
}