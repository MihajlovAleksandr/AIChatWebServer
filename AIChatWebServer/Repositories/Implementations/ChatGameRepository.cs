using AIChatWebServer.Models.Chats.RandomChat;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;
using System.Data;

namespace AIChatWebServer.Repositories.Implementations;

public sealed class ChatGameRepository
    : BaseRepository, IChatGameRepository
{
    private readonly ILogger<ChatGameRepository> _logger;
    private readonly IConfiguration _configuration;
    private readonly NpgsqlConnection? _conn;
    private readonly NpgsqlTransaction? _tx;
    private readonly bool _isExternalConnection;

    public ChatGameRepository(
        IConfiguration configuration,
        ILogger<ChatGameRepository> logger) : base(configuration)
    {
        _configuration = configuration;
        _logger = logger;
        _isExternalConnection = false;
    }

    private ChatGameRepository(
        IConfiguration configuration,
        ILogger<ChatGameRepository> logger,
        NpgsqlConnection conn,
        NpgsqlTransaction tx) : base(configuration) 
    {
        _configuration = configuration;
        _logger = logger;
        _conn = conn;
        _tx = tx;
        _isExternalConnection = true;
    }

    public IChatGameRepository WithTransaction(NpgsqlConnection conn, NpgsqlTransaction tx)
    {
        return new ChatGameRepository(_configuration, _logger, conn, tx);
    }

    public async Task<Guid> CreateAsync(
        Guid chatId,
        Guid guesserUserChatId,
        Guid opponentUserChatId,
        AiRole opponentAiRole,
        CancellationToken ct = default)
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

            await using var createCmd = new NpgsqlCommand(
                ChatGameQueries.CreateSession,
                connection,
                transaction);

            createCmd.Parameters.AddWithValue("@ChatId", chatId);

            var sessionId = (Guid)(await createCmd.ExecuteScalarAsync(ct))!;

            await using (var cmd = new NpgsqlCommand(
                ChatGameQueries.InsertParticipant,
                connection,
                transaction))
            {
                cmd.Parameters.AddWithValue("@SessionId", sessionId);
                cmd.Parameters.AddWithValue("@UserChatId", guesserUserChatId);
                cmd.Parameters.AddWithValue("@GameRole", ToDb(GameRole.Guesser));
                cmd.Parameters.AddWithValue("@AiRole", DBNull.Value);

                await cmd.ExecuteNonQueryAsync(ct);
            }

            await using (var cmd = new NpgsqlCommand(
                ChatGameQueries.InsertParticipant,
                connection,
                transaction))
            {
                cmd.Parameters.AddWithValue("@SessionId", sessionId);
                cmd.Parameters.AddWithValue("@UserChatId", opponentUserChatId);
                cmd.Parameters.AddWithValue("@GameRole", ToDb(GameRole.Opponent));
                cmd.Parameters.AddWithValue("@AiRole", ToDb(opponentAiRole));

                await cmd.ExecuteNonQueryAsync(ct);
            }

            if (ownsTransaction && transaction != null)
            {
                await transaction.CommitAsync(ct);
            }

            return sessionId;
        }
        catch (Exception ex)
        {
            if (ownsTransaction && transaction != null)
            {
                await transaction.RollbackAsync(ct);
            }

            _logger.LogError(ex, "Failed to create game session for ChatId={ChatId}", chatId);
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

    public async Task<ChatGameSession?> GetByIdAsync(
        Guid sessionId,
        CancellationToken ct = default)
    {
        return await GetInternalAsync(
            ChatGameQueries.GetSessionWithParticipants,
            ("@SessionId", sessionId),
            ct);
    }

    public async Task<ChatGameSession?> GetByChatIdAsync(
        Guid chatId,
        CancellationToken ct = default)
    {
        return await GetInternalAsync(
            ChatGameQueries.GetSessionByChatId,
            ("@ChatId", chatId),
            ct);
    }

    public async Task SetResultAsync(
        Guid sessionId,
        AiRole guessedAiRole,
        CancellationToken ct = default)
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
                ChatGameQueries.SetResult,
                connection,
                _tx);

            cmd.Parameters.AddWithValue("@SessionId", sessionId);
            cmd.Parameters.AddWithValue("@GuessedAiRole", ToDb(guessedAiRole));

            await cmd.ExecuteNonQueryAsync(ct);
        }
        finally
        {
            if (ownsConnection && connection != null)
            {
                await connection.DisposeAsync();
            }
        }
    }

    public async Task<bool?> GetResultAsync(
        Guid sessionId,
        CancellationToken ct = default)
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
                ChatGameQueries.GetResult,
                connection,
                _tx);

            cmd.Parameters.AddWithValue("@SessionId", sessionId);

            var result = await cmd.ExecuteScalarAsync(ct);

            return result as bool?;
        }
        finally
        {
            if (ownsConnection && connection != null)
            {
                await connection.DisposeAsync();
            }
        }
    }

    private async Task<ChatGameSession?> GetInternalAsync(
        string query,
        (string, object) parameter,
        CancellationToken ct)
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

            await using var cmd = new NpgsqlCommand(query, connection, _tx);
            cmd.Parameters.AddWithValue(parameter.Item1, parameter.Item2);

            await using var reader = await cmd.ExecuteReaderAsync(ct);

            if (!await reader.ReadAsync(ct))
                return null;

            var sessionId = reader.GetGuid("session_id");
            var chatId = reader.GetGuid("chat_id");

            var guessed = reader.IsDBNull("guessed_ai_role")
                ? (AiRole?)null
                : FromDbAiRole(reader.GetString("guessed_ai_role"));

            var participants = new List<ChatGameParticipant>();

            do
            {
                participants.Add(new ChatGameParticipant(
                    reader.GetGuid("participant_id"),
                    reader.GetGuid("user_chat_id"),
                    FromDbGameRole(reader.GetString("game_role")),
                    reader.IsDBNull("ai_role")
                        ? null
                        : FromDbAiRole(reader.GetString("ai_role"))
                ));
            }
            while (await reader.ReadAsync(ct));

            return new ChatGameSession(
                sessionId,
                chatId,
                guessed,
                participants);
        }
        finally
        {
            if (ownsConnection && connection != null)
            {
                await connection.DisposeAsync();
            }
        }
    }

    private static string ToDb(GameRole role) =>
        role switch
        {
            GameRole.Guesser => "GUESSER",
            GameRole.Opponent => "OPPONENT",
            _ => throw new ArgumentOutOfRangeException(nameof(role))
        };

    private static string ToDb(AiRole role) =>
        role switch
        {
            AiRole.RealAi => "REAL_AI",
            AiRole.FakeAi => "FAKE_AI",
            _ => throw new ArgumentOutOfRangeException(nameof(role))
        };

    private static GameRole FromDbGameRole(string value) =>
        value switch
        {
            "GUESSER" => GameRole.Guesser,
            "OPPONENT" => GameRole.Opponent,
            _ => throw new ArgumentOutOfRangeException(nameof(value))
        };

    private static AiRole FromDbAiRole(string value) =>
        value switch
        {
            "REAL_AI" => AiRole.RealAi,
            "FAKE_AI" => AiRole.FakeAi,
            _ => throw new ArgumentOutOfRangeException(nameof(value))
        };
}