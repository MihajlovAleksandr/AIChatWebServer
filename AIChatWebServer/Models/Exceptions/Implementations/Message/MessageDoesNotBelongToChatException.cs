using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Message
{
    public class MessageDoesNotBelongToChatException(Guid messageId, Guid chatId)
        : ApiExceptionBase(
            403,
            MessageErrors.MessageDoesNotBelongToChat,
            $"Message {messageId} does not belong to chat {chatId}")
    {
    }
}
