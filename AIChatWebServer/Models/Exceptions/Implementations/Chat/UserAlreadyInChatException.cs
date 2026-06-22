using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat
{
    public class UserAlreadyInChatException(Guid chatId, Guid userId)
        : ApiExceptionBase(
            409,
            ChatErrors.UserAlreadyInChat,
            $"User {userId} is already a member of chat {chatId}")
    {
    }
}
