using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.User
{
    public class AuthIdentityAlreadyExistsException(Guid userId, string providerCode) : ApiExceptionBase(409, UserErrors.AuthIdentityAlreadyExists, $"User {userId} already has auth identity {providerCode}")
    {
    }
}
