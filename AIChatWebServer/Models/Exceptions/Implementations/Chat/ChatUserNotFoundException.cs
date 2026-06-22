using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat
{
    public class ChatUserNotFoundException(Guid chatUserId) : ApiExceptionBase(404, ChatErrors.ChatUserNotFound, $"ChatUser {chatUserId} was not found")
    {
    }
}
