using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.Chat;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.ChatPermissions
{
    public class CallSettingsChatRule : ConversationActionRuleBase<CallAction>
    {
        public override void Validate(Chat chat, CallAction action)
        {
            if (!chat.Settings.Calls.CallEnabled || !chat.Settings.Calls.VideoEnabled && action.IsVideo)
            {
                throw new ChatCallsDisabledByChatSettingsException(chat.Id);
            }
        }
    }
}
