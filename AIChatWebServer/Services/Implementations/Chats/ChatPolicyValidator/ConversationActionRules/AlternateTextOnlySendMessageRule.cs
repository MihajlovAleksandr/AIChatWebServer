using AIChatWebServer.Models.Chats;
namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.ConversationActionRules
{
    public class AlternateTextOnlySendMessageRule : AlternateSendMessageRule
    {
        private readonly TextOnlySendMessageRule _textOnlyRule = new TextOnlySendMessageRule();

        public override void Validate(Chat chat, SendMessageAction action)
        {
            _textOnlyRule.Validate(chat, action);
            base.Validate(chat, action);
        }
    }
}
