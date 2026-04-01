using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.PolicyExceptions
{
    public class MessageAttachmentForbiddenException(Guid chatId) :
        ChatPolicyException(
            ChatErrors.SendMessageAttachmentForbidden, 
            $"Chat {chatId} does not allow send message attachment according to its policy.")
    {
    }
}
