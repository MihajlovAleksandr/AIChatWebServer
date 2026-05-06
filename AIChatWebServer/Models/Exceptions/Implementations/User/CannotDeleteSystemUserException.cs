using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.User
{
    public sealed class CannotDeleteSystemUserException(Guid userId)
        : ApiExceptionBase(400, UserErrors.CannotDeleteSystemUser, $"System user {userId} cannot be deleted")
    {
    }
}
