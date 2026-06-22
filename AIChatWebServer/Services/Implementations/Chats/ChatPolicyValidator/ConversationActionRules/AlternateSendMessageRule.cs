using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.PolicyExceptions;
using System.Collections.Concurrent;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.ConversationActionRules
{
    public class AlternateSendMessageRule : ConversationActionRuleBase<SendMessageAction>
    {
        private readonly ConcurrentDictionary<Chat, Guid> _messageQueue = new ConcurrentDictionary<Chat, Guid>();

        public override void Validate(Chat chat, SendMessageAction action)
        {
            _messageQueue.AddOrUpdate(chat, key =>
            {
                return action.UserId;
            },
            (key, value) =>
            {
                if (value == action.UserId)
                    throw new ConsecutiveMessagesFromSameUserForbiddenByPolicyException(chat.Id, action.UserId);
                return action.UserId;
            });
        }
    }
}
