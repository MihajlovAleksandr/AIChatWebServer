using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User
{
    public sealed class ChatEndForbiddenForNonOwnerException(
        Guid chatId,
        Guid actingUserId)
        : ChatUserSettingsException(
            ChatErrors.ChatEndForbiddenForNonOwner,
            $"User {actingUserId} cannot end chat {chatId} because only the owner is allowed to perform this action.")
    {
    }
}