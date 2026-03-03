using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat
{
    public class UserDoesNotBelongToChatException(Guid chatId, Guid userId)
        : ApiExceptionBase(
            403,
            ChatErrors.UserDoesNotBelongToChat,
            $"User {userId} does not belong to chat {chatId}")
    {
    }
}
