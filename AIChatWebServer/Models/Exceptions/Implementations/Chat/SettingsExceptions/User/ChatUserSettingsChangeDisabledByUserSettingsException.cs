using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User
{
    public sealed class ChatUserSettingsChangeDisabledByUserSettingsException(
        Guid chatId,
        Guid userId)
        : ChatUserSettingsException(
            ChatErrors.ChatUserSettingsChangeDisabledByUserSettings,
            $"User {userId} cannot change user settings in chat {chatId} because this action is disabled in their settings.")
    {
    }
}