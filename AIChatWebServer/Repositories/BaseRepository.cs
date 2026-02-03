using Npgsql;

namespace AIChatWebServer.Repositories
{
    public abstract class BaseRepository
    {
        private readonly string _connectionString;
        protected BaseRepository()
        {
            _connectionString =
                System.Configuration.ConfigurationManager
                    .ConnectionStrings["MainDatabase"]
                    ?.ConnectionString
                ?? throw new InvalidOperationException(
                    "Connection string 'MainDatabase' not found in app.config");
        }
        protected NpgsqlConnection GetConnection()
        {
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        protected async Task<NpgsqlConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            return connection;
        }
    }
}
