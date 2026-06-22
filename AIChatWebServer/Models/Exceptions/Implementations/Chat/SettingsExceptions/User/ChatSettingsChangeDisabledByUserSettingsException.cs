using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User
{
    public class ChatSettingsChangeDisabledByUserSettingsException(
        Guid chatId,
        Guid userId)
        : ChatUserSettingsException(
            ChatErrors.ChatUserSettingsChangeDisabledByUserSettings,
            $"User {userId} cannot change chat settings in chat {chatId} because this action is disabled in their settings.");
}
