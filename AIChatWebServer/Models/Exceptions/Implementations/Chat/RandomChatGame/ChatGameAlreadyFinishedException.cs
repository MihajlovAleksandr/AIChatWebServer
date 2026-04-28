using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.RandomChatGame
{
    public sealed class ChatGameAlreadyFinishedException(Guid sessionId)
        : ApiExceptionBase(
            409,
            ChatGameErrors.GameAlreadyFinished,
            $"Game session {sessionId} already finished")
    {
    }
}
