using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.PolicyExceptions
{
    public sealed class ChatSettingsModificationForbiddenException(Guid chatId)
        : ChatPolicyException(
            ChatErrors.ChatSettingsModificationForbidden,
            $"Chat {chatId} does not allow modifying its settings according to its policy.");
}
