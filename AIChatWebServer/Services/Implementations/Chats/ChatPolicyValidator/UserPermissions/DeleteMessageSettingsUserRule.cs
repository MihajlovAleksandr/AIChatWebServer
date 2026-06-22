using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.UserPermissions
{
    public class DeleteMessageSettingsUserRule : ConversationActionRuleBase<DeleteMessageAction>
    {
        public override void Validate(Chat chat, DeleteMessageAction action)
        {
            if (!chat.UsersWithData.TryGetValue(action.UserId, out ChatUserData? chatUserData))
            {
                throw new UserDoesNotBelongToChatException(chat.Id, action.UserId);
            }

            bool isOwnMessage = action.Message.UserId == action.UserId;

            if (!chatUserData.UserSettings.Messages.DeleteOwnMessagesEnabled
                && isOwnMessage)
                throw new MessageDeletionDisabledByUserSettingsException(chat.Id, action.UserId, action.Message.Id);

            if(!chatUserData.UserSettings.Messages.DeleteOtherMessagesEnabled
                && !isOwnMessage)
                throw new MessageDeletionForbiddenForOthersByUserSettingsException(action.Message.ChatId, action.UserId, action.Message.Id);
        }
    }
}
