using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User
{
    public sealed class ChatMessageImagesDisabledByUserSettingsException(Guid chatId, Guid userId)
        : ChatSettingsException(
            ChatErrors.ChatMessageImagesDisabledByUserSettings,
            $"User {userId} cannot send image attachments in chat {chatId} because it is disabled in the user settings.");
}