using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User
{
    public class ChatUserAdditionDisabledByUserSettingsException(Guid chatId, Guid userId) :
        ChatUserSettingsException(
            ChatErrors.ChatUserAdditionDisabledByUserSettings,
            $"Chat {chatId} does not allow adding users because it is disabled in the user {userId} settings.");
}
