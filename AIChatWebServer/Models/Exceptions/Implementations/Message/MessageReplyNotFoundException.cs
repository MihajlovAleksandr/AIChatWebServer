using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Message
{
    public class MessageReplyNotFoundException(Guid replyMessageId)
        : ApiExceptionBase(
            404,
            MessageErrors.MessageReplyNotFound,
            $"Reply message {replyMessageId} was not found")
    {
    }
}