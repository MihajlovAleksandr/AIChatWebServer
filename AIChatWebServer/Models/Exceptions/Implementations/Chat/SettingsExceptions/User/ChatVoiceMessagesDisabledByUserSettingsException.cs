using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User
{
    public sealed class ChatVoiceMessagesDisabledByUserSettingsException(Guid chatId, Guid userId)
        : ChatSettingsException(
            ChatErrors.ChatVoiceMessagesDisabledByUserSettings,
            $"User {userId} cannot send voice messages in chat {chatId} because it is disabled in the user settings.");
}