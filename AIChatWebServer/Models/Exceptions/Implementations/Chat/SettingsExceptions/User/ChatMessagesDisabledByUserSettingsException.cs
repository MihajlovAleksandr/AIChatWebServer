using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User
{
    public sealed class ChatMessagesDisabledByUserSettingsException(Guid chatId, Guid userId)
        : ChatSettingsException(
            ChatErrors.ChatMessagesDisabledByUserSettings,
            $"User {userId} cannot send messages in chat {chatId} because it is disabled in the user settings.");
}