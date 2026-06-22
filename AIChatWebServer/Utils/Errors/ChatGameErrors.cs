namespace AIChatWebServer.Utils.Errors;

public sealed class ChatGameErrors : ErrorCode
{
    private ChatGameErrors(string code) : base(code)
    {
    }

    public static readonly IErrorCode GameNotFound =
        new ChatGameErrors("GAME_NOT_FOUND");

    public static readonly IErrorCode GameAlreadyExists =
        new ChatGameErrors("GAME_ALREADY_EXISTS");

    public static readonly IErrorCode GameAlreadyFinished =
        new ChatGameErrors("GAME_ALREADY_FINISHED");

    public static readonly IErrorCode GameNotFinished =
        new ChatGameErrors("GAME_NOT_FINISHED");

    public static readonly IErrorCode InvalidPlayers =
        new ChatGameErrors("INVALID_PLAYERS");

    public static readonly IErrorCode InvalidGuess =
        new ChatGameErrors("INVALID_GUESS");
}