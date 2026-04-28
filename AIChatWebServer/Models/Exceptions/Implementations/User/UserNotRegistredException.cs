using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.User
{
    public class UserNotRegistredException(Guid userId) : ApiExceptionBase(409, UserErrors.UserNotRegistred, $"User {userId} not registred")
    {
    }
}
