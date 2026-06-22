using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.RandomChatGame
{
    public sealed class ChatGameNotFinishedException(Guid sessionId)
        : ApiExceptionBase(
            400,
            ChatGameErrors.GameNotFinished,
            $"Game session {sessionId} is not finished")
    {
    }
}
