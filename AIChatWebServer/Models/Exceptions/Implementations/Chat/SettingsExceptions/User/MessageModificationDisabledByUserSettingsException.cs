using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User
{
    public sealed class MessageModificationDisabledByUserSettingsException(
        Guid chatId,
        Guid userId,
        Guid messageId)
        : ChatUserSettingsException(
            ChatErrors.ChatMessageModificationDisabledByUserSettings,
            $"User {userId} cannot modify message {messageId} in chat {chatId} because this action is disabled in their settings.");
}