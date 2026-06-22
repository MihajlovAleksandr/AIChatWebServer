using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions
{
    public abstract class ChatUserSettingsException(IErrorCode errorCode, string message) : ApiExceptionBase(403, errorCode, message);
}
