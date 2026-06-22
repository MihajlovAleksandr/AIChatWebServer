using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.Matchmaking
{
    public class UserNotSearchingChatException(Guid userId)
        : ApiExceptionBase(
            404,
            MatchmakingErrors.UserNotSearchingChat,
            $"User {userId} not searching chat")
    {
    }
}
