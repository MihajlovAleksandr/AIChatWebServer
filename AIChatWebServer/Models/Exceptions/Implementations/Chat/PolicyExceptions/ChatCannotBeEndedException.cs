using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.PolicyExceptions
{
    public sealed class ChatCannotBeEndedException(Guid chatId)
        : ChatPolicyException(
            ChatErrors.ChatCannotBeEnded,
            $"Chat {chatId} cannot be ended according to its policy.");
}
