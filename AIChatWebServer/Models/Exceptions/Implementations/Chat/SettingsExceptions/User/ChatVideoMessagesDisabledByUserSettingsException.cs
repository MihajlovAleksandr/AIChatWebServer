using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User
{
    public sealed class ChatVideoMessagesDisabledByUserSettingsException(Guid chatId, Guid userId)
        : ChatSettingsException(
            ChatErrors.ChatVideoMessagesDisabledByUserSettings,
            $"User {userId} cannot send video messages in chat {chatId} because it is disabled in the user settings.");
}