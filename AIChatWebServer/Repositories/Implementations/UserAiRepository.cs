using AIChatWebServer.Models.AI;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class UserAiRepository : BaseRepository, IUserAiRepository
    {
        private readonly ILogger<UserAiRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly NpgsqlConnection? _conn;
        private readonly NpgsqlTransaction? _tx;
        private readonly bool _isExternalConnection;

        public UserAiRepository(IConfiguration configuration,
            ILogger<UserAiRepository> logger) : base(configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _isExternalConnection = false;
        }

        private UserAiRepository(IConfiguration configuration,
            ILogger<UserAiRepository> logger,
            NpgsqlConnection conn,
            NpgsqlTransaction tx) : base(configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _conn = conn;
            _tx = tx;
            _isExternalConnection = true;
        }

        public IUserAiRepository WithTransaction(NpgsqlConnection conn, NpgsqlTransaction tx)
        {
            return new UserAiRepository(_configuration, _logger, conn, tx);
        }

        public async Task CreateAsync(UserAiModel userAiModel, CancellationToken ct = default)
        {
            await ExecuteAsync(
                UserAiModelQueries.Insert,
                ct,
                ("@id", userAiModel.Id),
                ("@user_id", userAiModel.UserId),
                ("@model", (int)userAiModel.Model),
                ("@payment_item_id", userAiModel.PaymentItemId),
                ("@created_at", userAiModel.CreatedAt)
            );
        }

        public async Task<UserAiModel?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var list = await ReadAsync(
                UserAiModelQueries.GetById,
                ct,
                ("@id", id));

            return list.FirstOrDefault();
        }

        public async Task<UserAiModel?> GetByUserIdAndModelAsync(Guid userId, AIModel model, CancellationToken ct = default)
        {
            var list = await ReadAsync(
                UserAiModelQueries.GetByUserIdAndModel,
                ct,
                ("@user_id", userId),
                ("@model", (int)model));

            return list.FirstOrDefault();
        }

        public async Task<bool> ExistsByUserIdAndModelAsync(Guid userId, AIModel model, CancellationToken ct = default)
        {
            NpgsqlConnection? connection = null;
            bool ownsConnection = false;

            try
            {
                if (_isExternalConnection)
                {
                    connection = _conn;
                }
                else
                {
                    connection = await GetConnectionAsync(ct);
                    ownsConnection = true;
                }

                await using var cmd = new NpgsqlCommand(
                    UserAiModelQueries.ExistsByUserIdAndModel,
                    connection,
                    _tx);

                cmd.Parameters.AddWithValue("@user_id", userId);
                cmd.Parameters.AddWithValue("@model", (int)model);

                var result = await cmd.ExecuteScalarAsync(ct);

                return result is bool exists && exists;
            }
            finally
            {
                if (ownsConnection && connection != null)
                {
                    await connection.DisposeAsync();
                }
            }
        }

        public async Task<IReadOnlyList<UserAiModel>> GetAllByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await ReadAsync(
                UserAiModelQueries.GetAllByUserId,
                ct,
                ("@user_id", userId));
        }

        private async Task<List<UserAiModel>> ReadAsync(
            string sql,
            CancellationToken ct,
            params (string, object)[] parameters)
        {
            NpgsqlConnection? connection = null;
            bool ownsConnection = false;

            try
            {
                if (_isExternalConnection)
                {
                    connection = _conn;
                }
                else
                {
                    connection = await GetConnectionAsync(ct);
                    ownsConnection = true;
                }

                await using var cmd = new NpgsqlCommand(sql, connection, _tx);

                AddParams(cmd, parameters);

                await using var reader = await cmd.ExecuteReaderAsync(ct);

                var list = new List<UserAiModel>();

                while (await reader.ReadAsync(ct))
                {
                    list.Add(Map(reader));
                }

                return list;
            }
            finally
            {
                if (ownsConnection && connection != null)
                {
                    await connection.DisposeAsync();
                }
            }
        }

        private async Task ExecuteAsync(
            string sql,
            CancellationToken ct,
            params (string, object)[] parameters)
        {
            NpgsqlConnection? connection = null;
            NpgsqlTransaction? transaction = null;
            bool ownsConnection = false;
            bool ownsTransaction = false;

            try
            {
                if (_isExternalConnection)
                {
                    connection = _conn;
                    transaction = _tx;
                }
                else
                {
                    connection = await GetConnectionAsync(ct);
                    transaction = await connection.BeginTransactionAsync(ct);
                    ownsConnection = true;
                    ownsTransaction = true;
                }

                await using var cmd = new NpgsqlCommand(sql, connection, transaction);

                AddParams(cmd, parameters);

                await cmd.ExecuteNonQueryAsync(ct);

                if (ownsTransaction && transaction != null)
                {
                    await transaction.CommitAsync(ct);
                }
            }
            catch (Exception ex)
            {
                if (ownsTransaction && transaction != null)
                {
                    await transaction.RollbackAsync(ct);
                }

                _logger.LogError(ex, "Failed to execute query: {Sql}", sql);
                throw;
            }
            finally
            {
                if (ownsConnection && connection != null)
                {
                    await connection.DisposeAsync();
                }
            }
        }

        private static void AddParams(NpgsqlCommand cmd, params (string, object)[] parameters)
        {
            foreach (var (name, value) in parameters)
            {
                cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
            }
        }

        private static UserAiModel Map(NpgsqlDataReader reader)
        {
            var modelValue = reader.GetInt32(reader.GetOrdinal("model"));

            if (!Enum.IsDefined(typeof(AIModel), modelValue))
            {
                throw new InvalidOperationException($"Unknown AI model value: {modelValue}");
            }

            return new UserAiModel
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                UserId = reader.GetGuid(reader.GetOrdinal("user_id")),
                Model = (AIModel)modelValue,
                PaymentItemId = reader.GetGuid(reader.GetOrdinal("payment_item_id")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
            };
        }
    }
}