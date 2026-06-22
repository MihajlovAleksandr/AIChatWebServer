namespace AIChatWebServer.Repositories.Constants;

public static class ChatGameQueries
{
    public const string CreateSession = """
        INSERT INTO chat_game_session (chat_id)
        VALUES (@ChatId)
        RETURNING id;
        """;

    public const string InsertParticipant = """
        INSERT INTO chat_game_participants (
            game_session_id,
            user_chat_id,
            game_role,
            ai_role
        )
        VALUES (@SessionId, @UserChatId, @GameRole, @AiRole);
        """;

    public const string GetSessionWithParticipants = """
        SELECT 
            s.id as session_id,
            s.chat_id,
            s.guessed_ai_role,

            p.id as participant_id,
            p.user_chat_id,
            p.game_role,
            p.ai_role

        FROM chat_game_session s
        JOIN chat_game_participants p
            ON p.game_session_id = s.id

        WHERE s.id = @SessionId;
        """;

    public const string GetSessionByChatId = """
        SELECT 
            s.id as session_id,
            s.chat_id,
            s.guessed_ai_role,

            p.id as participant_id,
            p.user_chat_id,
            p.game_role,
            p.ai_role

        FROM chat_game_session s
        JOIN chat_game_participants p
            ON p.game_session_id = s.id

        WHERE s.chat_id = @ChatId;
        """;

    public const string SetResult = """
        UPDATE chat_game_session
        SET guessed_ai_role = @GuessedAiRole
        WHERE id = @SessionId;
        """;

    public const string GetResult = """
        SELECT 
            CASE 
                WHEN s.guessed_ai_role = p.ai_role THEN true
                ELSE false
            END AS is_correct
        FROM chat_game_session s
        JOIN chat_game_participants p
            ON p.game_session_id = s.id
           AND p.game_role = 'OPPONENT'
        WHERE s.id = @SessionId;
        """;
}