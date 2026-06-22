using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.User
{
    public class InvalidServerException(Guid userId) : ApiExceptionBase(409, UserErrors.InvalidServerType, $"Invalid server type. Server: {userId}")
    {
    }
}
