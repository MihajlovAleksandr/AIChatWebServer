using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User
{
    public sealed class MessageDeletionForbiddenForOthersByUserSettingsException(
        Guid chatId,
        Guid userId,
        Guid messageId)
        : ChatUserSettingsException(
            ChatErrors.ChatMessageDeletionForbiddenForOthersByUserSettings,
            $"User {userId} cannot delete message {messageId} in chat {chatId} because deleting others' messages is disabled in their settings.");
}