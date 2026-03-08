using AIChatWebServer.Models.Chats;
using AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.ChatPermissions;
using AIChatWebServer.Services.Interfaces.Chats;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator
{
    public class ChatSettingsFactory : IChatSettingsFactory
    {
        public IChatRulesValidator Create()
        {
            return new ChatRulesValidator(new Dictionary<Type, IConversationActionRule>
            {
                { typeof(AddUserAction), new AddUserSettingsChatRule() },
                { typeof(CallAction), new CallSettingsChatRule() },
                { typeof(InviteUserToChatAction), new InviteUserSettingsChatRule() }
            });
        }
    }
}
