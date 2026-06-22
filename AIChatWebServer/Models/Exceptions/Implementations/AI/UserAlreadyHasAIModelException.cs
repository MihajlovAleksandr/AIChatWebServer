using AIChatWebServer.Models.AI;
using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.AI
{
    public class UserAlreadyHasAIModelException(Guid userId, AIModel model)
        : ApiExceptionBase(
            409,
            AIModelErrors.UserAlreadyHasAIModel,
            $"User {userId} already has AI model {model}");
}
