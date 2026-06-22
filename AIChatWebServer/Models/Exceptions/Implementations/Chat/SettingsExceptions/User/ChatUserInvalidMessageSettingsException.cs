using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User
{
    public sealed class ChatUserInvalidMessageSettingsException(
        Guid chatId,
        Guid actingUserId,
        Guid targetUserId,
        string reason)
        : ChatUserSettingsException(
            ChatErrors.ChatUserInvalidMessageSettings,
            $"User {actingUserId} attempted to set invalid message settings for user {targetUserId} in chat {chatId}. Reason: {reason}")
    {
    }
}