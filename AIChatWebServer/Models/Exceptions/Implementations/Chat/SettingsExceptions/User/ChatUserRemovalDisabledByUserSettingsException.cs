using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User
{
    public class ChatUserRemovalDisabledByUserSettingsException(Guid chatId, Guid userId) :
        ChatUserSettingsException(
            ChatErrors.ChatUserRemovalDisabledByUserSettings,
            $"Chat {chatId} does not allow removing users because it is disabled in the user {userId} settings.");
}
