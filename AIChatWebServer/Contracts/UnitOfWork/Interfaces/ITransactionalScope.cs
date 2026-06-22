using Npgsql;

namespace AIChatWebServer.Contracts.UnitOfWork.Interfaces
{
    public interface ITransactionalScope<T> where T : ITransactionalScope<T>
    {
        T WithTransaction(NpgsqlConnection conn, NpgsqlTransaction tx);
    }
}
