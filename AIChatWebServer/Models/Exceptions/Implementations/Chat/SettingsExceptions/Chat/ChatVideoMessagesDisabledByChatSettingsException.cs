using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.Chat
{
    public sealed class ChatVideoMessagesDisabledByChatSettingsException(Guid chatId)
        : ChatSettingsException(
            ChatErrors.ChatVideoMessagesDisabledByChatSettings,
            $"Chat {chatId} does not allow sending video messages because it is disabled in the chat settings.");
}