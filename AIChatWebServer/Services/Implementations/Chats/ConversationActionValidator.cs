using AIChatWebServer.Models.Chats;
using AIChatWebServer.Services.Interfaces.Chats;

namespace AIChatWebServer.Services.Implementations.Chats
{
    public class ConversationActionValidator(
        IChatPolicyFactory chatPolicyFactory,
        IChatSettingsFactory chatSettingsFactory,
        IUserSettingsFactory userSettingsFactory
    ) : IConversationActionValidator
    {
        private readonly IChatPolicyFactory _chatPolicyFactory = chatPolicyFactory;
        private readonly IChatSettingsFactory _chatSettingsFactory = chatSettingsFactory;
        private readonly IUserSettingsFactory _userSettingsFactory = userSettingsFactory;

        public void Validate(Chat chat, ConversationAction chatAction)
        {
            _chatPolicyFactory.Create(chat.Type).Validate(chat, chatAction);
            _chatSettingsFactory.Create().Validate(chat, chatAction);
            _userSettingsFactory.Create().Validate(chat, chatAction);
        }
    }
}
