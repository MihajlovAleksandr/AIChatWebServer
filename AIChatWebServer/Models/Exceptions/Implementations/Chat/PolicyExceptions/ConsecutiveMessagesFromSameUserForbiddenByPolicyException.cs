using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.PolicyExceptions
{
    public sealed class ConsecutiveMessagesFromSameUserForbiddenByPolicyException(Guid chatId, Guid userId)
        : ChatPolicyException(
            ChatErrors.ConsecutiveMessagesFromSameUserForbiddenByPolicy,
            $"Chat {chatId} policy forbids sending consecutive messages from the same user {userId}. Users must send messages in turn.");
}