using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Message
{
    public class CannotEditForeignMessageException(Guid messageId, Guid userId)
        : ApiExceptionBase(
            403,
            MessageErrors.CannotEditForeignMessage,
            $"Attempt to edit a message {messageId} that does not belong to user {userId}")
    {
    }
}