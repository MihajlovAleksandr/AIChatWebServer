using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat
{
    public class UserNotInChatException(Guid chatId, Guid userId)
        : ApiExceptionBase(
            404,
            ChatErrors.UserNotInChat,
            $"User {userId} is not a member of chat {chatId}")
    {
    }
}
