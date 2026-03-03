using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat
{
    public class ChatNotFoundException(Guid chatId) : ApiExceptionBase(404, ChatErrors.ChatNotFound, $"Chat {chatId} was not found")
    {
    }
}
