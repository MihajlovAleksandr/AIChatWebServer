using AIChatWebServer.Models.Chats;
using AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.ConversationActionRules;
using AIChatWebServer.Services.Interfaces.Chats;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator
{
    public class ChatPolicyFactory : IChatPolicyFactory
    {
        AlternateTextOnlySendMessageRule alternateTextOnlySendMessageRule = new AlternateTextOnlySendMessageRule();

        public IChatRulesValidator Create(ChatType chatType)
        {
            return chatType switch
            {
                ChatType.AI => new ChatRulesValidator(new Dictionary<Type, IConversationActionRule>
                {
                    { typeof(AddUserAction), new AddUserForbiddenRule() },
                    { typeof(RemoveUserAction), new RemoveUserForbiddenRule() },
                    { typeof(CallAction), new CallsForbiddenRule() },
                    { typeof(EndChatAction), new EndChatValidationRule() },
                    { typeof(UpdateNameAction), new UpdateChatNameValidationRule() },
                    { typeof(InviteUserToChatAction), new InviteUserForbiddenRule() },
                    { typeof(StartSearchChatAction), new StartSearchForbiddenRule() },
                    { typeof(PrepareSendMessageAction), new TextOnlyPrepareSendMessageRule() },
                    { typeof(SendMessageAction), new TextOnlySendMessageRule() },
                    { typeof(EditMessageAction), new EditMessageForbiddenRule() },
                    { typeof(DeleteMessageAction), new DeleteMessageForbiddenRule() }
                }),

                ChatType.Human => new ChatRulesValidator(new Dictionary<Type, IConversationActionRule>
                {
                    { typeof(AddUserAction), new AddUserForbiddenRule() },
                    { typeof(RemoveUserAction), new RemoveUserForbiddenRule() },
                    { typeof(EndChatAction), new EndChatValidationRule() },
                    { typeof(UpdateNameAction), new UpdateChatNameValidationRule() },
                    { typeof(InviteUserToChatAction), new InviteUserForbiddenRule() },
                    { typeof(StartSearchChatAction), new StartSearchForbiddenRule() }
                }),

                ChatType.Random => new ChatRulesValidator(new Dictionary<Type, IConversationActionRule>
                {
                    { typeof(AddUserAction), new AddUserForbiddenRule() },
                    { typeof(RemoveUserAction), new RemoveUserForbiddenRule() },
                    { typeof(CallAction), new CallsForbiddenRule() },
                    { typeof(EndChatAction), new EndChatForbiddenRule() },
                    { typeof(ChangeChatSettingsAction), new SettingsModificationForbiddenRule() },
                    { typeof(UpdateNameAction), new UpdateChatNameValidationRule() },
                    { typeof(InviteUserToChatAction), new InviteUserForbiddenRule() },
                    { typeof(StartSearchChatAction), new StartSearchForbiddenRule() },
                    { typeof(PrepareSendMessageAction), new TextOnlyPrepareSendMessageRule() },
                    { typeof(SendMessageAction),  alternateTextOnlySendMessageRule },
                    { typeof(EditMessageAction), new EditMessageForbiddenRule() },
                    { typeof(DeleteMessageAction), new DeleteMessageForbiddenRule() }
                }),

                ChatType.Group => new ChatRulesValidator(
                    new Dictionary<Type, IConversationActionRule>
                    {
                        { typeof(EndChatAction), new EndChatValidationRule() },
                        { typeof(AddUserAction), new AddUserValidationRule() },
                        { typeof(UpdateNameAction), new UpdateChatNameValidationRule() }
                    }),

                _ => throw new NotSupportedException(),
            };
        }
    }
}