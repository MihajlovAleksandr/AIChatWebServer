using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations
{
    public class UserNotFoundException(Guid userId) :
        ApiExceptionBase(404, UserErrors.UserNotFound, $"User {userId} was not found")
    {
    }
}
