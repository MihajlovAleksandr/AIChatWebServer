using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.Chat
{
    public sealed class ChatMessageFilesDisabledByChatSettingsException(Guid chatId)
        : ChatSettingsException(
            ChatErrors.ChatMessageFilesDisabledByChatSettings,
            $"Chat {chatId} does not allow sending file attachments because it is disabled in the chat settings.");
}