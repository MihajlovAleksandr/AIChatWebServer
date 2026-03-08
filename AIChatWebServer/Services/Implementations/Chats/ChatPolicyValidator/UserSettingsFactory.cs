using AIChatWebServer.Models.Chats;
using AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.UserPermissions;
using AIChatWebServer.Services.Interfaces.Chats;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator
{
    public class UserSettingsFactory : IUserSettingsFactory
    {
        public IChatRulesValidator Create()
        {
            return new ChatRulesValidator(new Dictionary<Type, IConversationActionRule>
            {
                { typeof(AddUserAction), new AddUserSettingsUserRule() },
                { typeof(RemoveUserAction), new RemoveUserSettingsUserRule() },
                { typeof(ChangeChatSettingsAction), new ChangeChatSettingsUserRule() },
                { typeof(ChangeUserSettingsAction), new ChangeUserSettingsUserRule() },
                { typeof(CallAction), new StartCallSettingsUserRule() },
                { typeof(EndChatAction), new EndChatUserRule() },
                { typeof(InviteUserToChatAction), new InviteUserSettingsUserRule() }
            });
        }
    }
}
