using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.RandomChatGame
{
    public sealed class ChatGameInvalidPlayersException()
        : ApiExceptionBase(
            400,
            ChatGameErrors.InvalidPlayers,
            "Game must have exactly two different players")
    {
    }
}
