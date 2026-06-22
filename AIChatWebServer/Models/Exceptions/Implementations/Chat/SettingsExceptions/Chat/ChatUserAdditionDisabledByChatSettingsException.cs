using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.Chat
{
    public sealed class ChatUserAdditionDisabledByChatSettingsException(Guid chatId)
        : ChatSettingsException(
            ChatErrors.ChatUserAdditionDisabledByChatSettings,
            $"Chat {chatId} does not allow adding users because it is disabled in the chat settings.");
}
