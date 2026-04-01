using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.PolicyExceptions
{
    public sealed class MessageCannotBeModifiedException(Guid messageId, Guid chatId)
        : ChatPolicyException(
            ChatErrors.MessageModificationForbiddenByPolicy,
            $"Message {messageId} cannot be modified according to chat {chatId} policy.");
}