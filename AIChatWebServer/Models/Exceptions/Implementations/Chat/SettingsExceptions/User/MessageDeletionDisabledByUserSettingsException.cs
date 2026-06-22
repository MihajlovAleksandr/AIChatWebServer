using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User
{
    public sealed class MessageDeletionDisabledByUserSettingsException(
        Guid chatId,
        Guid userId,
        Guid messageId)
        : ChatUserSettingsException(
            ChatErrors.ChatMessageDeletionDisabledByUserSettings,
            $"User {userId} cannot delete message {messageId} in chat {chatId} because message deletion is disabled in their settings.");
}