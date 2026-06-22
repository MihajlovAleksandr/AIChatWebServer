using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Auth.Login
{
    public sealed class InvalidLoginProviderException(string email, string attemptedProvider) : ApiExceptionBase(
            400,
            UserErrors.InvalidLoginMethod,
            $"User with email {email} cannot authenticate using {attemptedProvider}")
    {
    }
}
