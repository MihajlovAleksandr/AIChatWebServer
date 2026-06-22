using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.RandomChatGame
{
    public sealed class ChatGameNotFoundException(Guid id)
        : ApiExceptionBase(
            404,
            ChatGameErrors.GameNotFound,
            $"Game session {id} not found")
    {
    }
}
