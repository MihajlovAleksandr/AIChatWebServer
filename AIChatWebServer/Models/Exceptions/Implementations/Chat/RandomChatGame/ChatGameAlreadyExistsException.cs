using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.RandomChatGame
{
    public sealed class ChatGameAlreadyExistsException(Guid chatId)
        : ApiExceptionBase(
            409,
            ChatGameErrors.GameAlreadyExists,
            $"Game already exists for chat {chatId}")
    {
    }
}
