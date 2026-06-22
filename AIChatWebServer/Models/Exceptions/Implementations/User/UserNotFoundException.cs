using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.User
{
    public sealed class UserNotFoundException : ApiExceptionBase
    {
        public UserNotFoundException(Guid userId) : base(404, UserErrors.UserNotFound, $"User {userId} was not found")
        {}

        public UserNotFoundException(string identifier) : base(404, UserErrors.UserNotFound, $"User with identifiern {identifier} was not found")
        {}
    }
}
