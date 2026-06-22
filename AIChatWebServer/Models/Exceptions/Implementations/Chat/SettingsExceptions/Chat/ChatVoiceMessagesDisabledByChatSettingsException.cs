using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.Chat
{
    public sealed class ChatVoiceMessagesDisabledByChatSettingsException(Guid chatId)
        : ChatSettingsException(
            ChatErrors.ChatVoiceMessagesDisabledByChatSettings,
            $"Chat {chatId} does not allow sending voice messages because it is disabled in the chat settings.");
}