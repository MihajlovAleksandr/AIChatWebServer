using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Message
{
    public class MessageNotFoundException(Guid messageId): ApiExceptionBase(404, MessageErrors.MessageNotFound, $"Message {messageId} was not found")
    {
    }
}
