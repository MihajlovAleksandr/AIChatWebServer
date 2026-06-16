using AIChatWebServer.Models.AI;
using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.AI
{
    public class AIModelNotAvailableForUserException(Guid userId, AIModel model)
       : ApiExceptionBase(
           403,
           AIModelErrors.AIModelNotAvailableForUser,
           $"AI model {model} is not available for user {userId}");
}
