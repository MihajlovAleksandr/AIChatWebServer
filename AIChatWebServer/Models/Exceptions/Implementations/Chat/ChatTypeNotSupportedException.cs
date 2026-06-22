using AIChatWebServer.Models.Chats;
using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat
{
    public class ChatTypeNotSupportedException(ChatType chatType)
        : ApiExceptionBase(
            400,
            ChatErrors.ChatTypeNotSupported,
            $"Chat type '{chatType}' is not supported")
    {
    }
}
