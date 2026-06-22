using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.Chat
{
    public sealed class ChatMessageImagesDisabledByChatSettingsException(Guid chatId)
        : ChatSettingsException(
            ChatErrors.ChatMessageImagesDisabledByChatSettings,
            $"Chat {chatId} does not allow sending image attachments because it is disabled in the chat settings.");
}