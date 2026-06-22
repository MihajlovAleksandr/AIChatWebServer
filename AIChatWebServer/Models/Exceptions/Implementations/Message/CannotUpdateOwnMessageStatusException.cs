using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Message
{
    public class CannotUpdateOwnMessageStatusException(Guid messageId, Guid userId)
        : ApiExceptionBase(
            400,
            MessageErrors.CannotUpdateOwnMessageStatus,
            $"Attempt to update status of own message {messageId} which is always considered read by user {userId}")
    {
    }
}