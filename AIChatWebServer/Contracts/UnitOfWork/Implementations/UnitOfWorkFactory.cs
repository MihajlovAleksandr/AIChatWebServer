using AIChatWebServer.Contracts.UnitOfWork.Interfaces;
using AIChatWebServer.Repositories;

namespace AIChatWebServer.Contracts.UnitOfWork.Implementations
{
    public sealed class UnitOfWorkFactory(IConfiguration configuration)
                : BaseRepository(configuration), IUnitOfWorkFactory
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