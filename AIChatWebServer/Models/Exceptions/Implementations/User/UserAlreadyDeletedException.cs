using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.User
{
    public sealed class UserAlreadyDeletedException(Guid userId)
        : ApiExceptionBase(409, UserErrors.UserAlreadyDeleted, $"User {userId} is already deleted")
    {
    }
}
