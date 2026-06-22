using AIChatWebServer.Contracts.UnitOfWork.Interfaces;
using Npgsql;

namespace AIChatWebServer.Contracts.UnitOfWork.Implementations
{
    public sealed class UnitOfWork(
        NpgsqlConnection conn,
        NpgsqlTransaction tx) : IUnitOfWork
    {
        private readonly NpgsqlConnection _conn = conn;
        private readonly NpgsqlTransaction _tx = tx;

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

        public T WithTransaction<T>(ITransactionalScope<T> repository) where T : ITransactionalScope<T>
        {
            return repository.WithTransaction(_conn, _tx);
        }
    }
}