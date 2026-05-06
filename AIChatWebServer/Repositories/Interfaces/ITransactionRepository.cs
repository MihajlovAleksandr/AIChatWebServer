using Npgsql;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface ITransactionRepository<T> where T : ITransactionRepository<T>
    {
        T WithTransaction(NpgsqlConnection conn, NpgsqlTransaction tx);
    }
}
