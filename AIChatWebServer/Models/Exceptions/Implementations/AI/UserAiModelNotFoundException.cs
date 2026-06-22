using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.AI
{
    public class UserAiModelNotFoundException(Guid userAiModelId)
    : ApiExceptionBase(
        404,
        AIModelErrors.UserAiModelNotFound,
        $"UserAiModel {userAiModelId} not found")
    {
    }
}