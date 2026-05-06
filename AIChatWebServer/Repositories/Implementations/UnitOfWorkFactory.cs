using AIChatWebServer.Repositories.Interfaces;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class UnitOfWorkFactory
                : BaseRepository, IUnitOfWorkFactory
    {

        public async Task<IUnitOfWork> CreateAsync(
            CancellationToken ct = default)
        {
            var conn =
                await GetConnectionAsync(ct);

            var tx =
                await conn.BeginTransactionAsync(ct);

            return new UnitOfWork(
                conn,
                tx);
        }
    }
}