using AIChatWebServer.Models.Chats.RandomChat;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;
using System.Data;

namespace AIChatWebServer.Repositories.Implementations;

public sealed class ChatGameRepository(
    ILogger<ChatGameRepository> logger)
    : BaseRepository, IChatGameRepository
{
    private readonly ILogger<ChatGameRepository> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<Guid> CreateAsync(
        Guid chatId,
        Guid guesserUserChatId,
        Guid opponentUserChatId,
        AiRole opponentAiRole,
        CancellationToken ct = default)
    {
        await using var connection = await GetConnectionAsync(ct);
        await using var transaction = await connection.BeginTransactionAsync(ct);

        try
        {
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

            await transaction.CommitAsync(ct);

            return sessionId;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);

            _logger.LogError(ex, "Failed to create game session for ChatId={ChatId}", chatId);
            throw;
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
        await using var connection = await GetConnectionAsync(ct);

        await using var cmd = new NpgsqlCommand(
            ChatGameQueries.SetResult,
            connection);

        cmd.Parameters.AddWithValue("@SessionId", sessionId);
        cmd.Parameters.AddWithValue("@GuessedAiRole", ToDb(guessedAiRole));

        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<bool?> GetResultAsync(
        Guid sessionId,
        CancellationToken ct = default)
    {
        await using var connection = await GetConnectionAsync(ct);

        await using var cmd = new NpgsqlCommand(
            ChatGameQueries.GetResult,
            connection);

        cmd.Parameters.AddWithValue("@SessionId", sessionId);

        var result = await cmd.ExecuteScalarAsync(ct);

        return result as bool?;
    }

    private async Task<ChatGameSession?> GetInternalAsync(
        string query,
        (string, object) parameter,
        CancellationToken ct)
    {
        await using var connection = await GetConnectionAsync(ct);

        await using var cmd = new NpgsqlCommand(query, connection);
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