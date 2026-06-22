using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.User
{
    public sealed class AuthIdentityNotFoundException(Guid userId, string providerCode) : ApiExceptionBase(404, UserErrors.AuthIdentityNotFound, 
        $"Auth identity with provider '{providerCode}' for user {userId} was not found")
    {
    }
}
