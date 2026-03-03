using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.PolicyExceptions
{
    public abstract class ChatPolicyException(IErrorCode errorCode, string message) : ApiExceptionBase(403, errorCode, message);
}
