using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User
{
    public sealed class ChatMessageFilesDisabledByUserSettingsException(Guid chatId, Guid userId)
        : ChatSettingsException(
            ChatErrors.ChatMessageFilesDisabledByUserSettings,
            $"User {userId} cannot send file attachments in chat {chatId} because it is disabled in the user settings.");
}