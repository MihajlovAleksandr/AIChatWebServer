using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Auth
{
    public sealed class UserMismatchException(Guid connectionUserId, Guid tokenUserId) : ApiExceptionBase(
            403,
            UserErrors.UserMismatch,
            $"Token user id {tokenUserId} does not match connection user id {connectionUserId}")
    {
    }
}
