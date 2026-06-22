using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.Matchmaking
{
    public class UserAlreadySearchingChatException(Guid userId)
        : ApiExceptionBase(
            409,
            MatchmakingErrors.UserAlreadySearchingChat,
            $"User {userId} is already searching for a chat")
    {
    }
}