using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Auth.Login
{
    public sealed class InvalidCredentialsException(Guid userId) : ApiExceptionBase(
            401,
            UserErrors.InvalidCredentials,
            $"Invalid credentials for user with id {userId}")
    {
    }
}
