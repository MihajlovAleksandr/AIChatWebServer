using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.Chat
{
    public sealed class ChatCallsDisabledByChatSettingsException(Guid chatId)
        : ChatSettingsException(
            ChatErrors.ChatCallsDisabledByChatSettings,
            $"Chat {chatId} does not allow calls because they are disabled in the chat settings.");
}
