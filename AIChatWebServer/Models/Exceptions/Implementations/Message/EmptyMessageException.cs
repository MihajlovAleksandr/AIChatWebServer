using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Message
{
    public class EmptyMessageException()
        : ApiExceptionBase(
            400,
            MessageErrors.EmptyMessage,
            "Attempt to send a message without text and attachments")
    {
    }
}
