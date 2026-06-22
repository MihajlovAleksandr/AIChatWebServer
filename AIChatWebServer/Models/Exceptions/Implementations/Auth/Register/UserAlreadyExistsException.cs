using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Auth.Register
{
    public sealed class UserAlreadyExistsException(string email) : ApiExceptionBase(
            409,
            RegisterErrors.UserAlreadyExists,
            $"User with email {email} already exists")
    {
    }
}
