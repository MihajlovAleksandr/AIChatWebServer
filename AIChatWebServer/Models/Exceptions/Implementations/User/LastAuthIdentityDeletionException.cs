using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.User
{
    public sealed class LastAuthIdentityDeletionException(Guid userId) : ApiExceptionBase(400, UserErrors.CannotDeleteLastAuthIdentity, $"User {userId} must have at least one authentication identity")
    {
    }
}
