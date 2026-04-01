using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.PolicyExceptions
{
    public sealed class MessageDeletionForbiddenByPolicyException(Guid messageId)
        : ChatPolicyException(
            ChatErrors.MessageDeletionForbiddenByPolicy,
            $"Message {messageId} cannot be deleted due to chat policy.");
}