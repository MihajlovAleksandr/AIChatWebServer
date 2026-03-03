using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.PolicyExceptions
{
    public sealed class ChatMembersModificationForbiddenException(Guid chatId)
        : ChatPolicyException(
            ChatErrors.ChatMembersModificationForbidden,
            $"Chat {chatId} does not allow modifying its members according to its policy.");
}
