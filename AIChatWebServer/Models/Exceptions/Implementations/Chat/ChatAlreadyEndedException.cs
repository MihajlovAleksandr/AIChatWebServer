using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat
{
    public class ChatAlreadyEndedException(Guid chatId)
        : ApiExceptionBase(
            409,
            ChatErrors.ChatAlreadyEnded,
            $"Chat {chatId} has already ended")
    {
    }
}
