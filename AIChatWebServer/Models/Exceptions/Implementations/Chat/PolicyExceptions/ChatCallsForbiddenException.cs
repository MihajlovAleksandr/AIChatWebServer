using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.PolicyExceptions
{
    public sealed class ChatCallsForbiddenException(Guid chatId)
        : ChatPolicyException(
            ChatErrors.ChatCallsForbidden,
            $"Chat {chatId} does not allow calls according to its policy.");
}
