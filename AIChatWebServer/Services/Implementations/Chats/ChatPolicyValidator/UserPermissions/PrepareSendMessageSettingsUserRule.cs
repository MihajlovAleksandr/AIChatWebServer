using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Chats.ValidateSettings;
using AIChatWebServer.Models.Exceptions.Implementations.Chat;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User;
using AIChatWebServer.Models.Files;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.UserPermissions
{
    public class PrepareSendMessageSettingsUserRule : ConversationActionRuleBase<PrepareSendMessageAction>
    {
        public override void Validate(Chat chat, PrepareSendMessageAction action)
        {
            if (chat == null)
                throw new ArgumentNullException(nameof(chat));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (!chat.UsersWithData.TryGetValue(action.UserId, out ChatUserData? chatUserData))
            {
                throw new UserDoesNotBelongToChatException(chat.Id, action.UserId);
            }

            ValidateMessageSettings(chat.Id, action, chatUserData.UserSettings.Messages);
        }

        private static void ValidateMessageSettings(
            Guid chatId,
            PrepareSendMessageAction action,
            UserMessageSettings settings)
        {
            if (!settings.MessagesEnabled)
            {
                throw new ChatMessagesDisabledByUserSettingsException(chatId, action.UserId);
            }

            if (action.AttachmentTypes == null || action.AttachmentTypes.Count == 0)
                return;

            foreach (var attachmentType in action.AttachmentTypes)
            {
                ValidateAttachment(chatId, action.UserId, settings, attachmentType);
            }
        }

        private static void ValidateAttachment(
            Guid chatId,
            Guid userId,
            UserMessageSettings settings,
            FileType attachmentType)
        {
            switch (attachmentType)
            {
                case FileType.MessageFile when !settings.MessageFilesEnabled:
                    throw new ChatMessageFilesDisabledByUserSettingsException(chatId, userId);

                case FileType.MessageImage when !settings.MessageImagesEnabled:
                    throw new ChatMessageImagesDisabledByUserSettingsException(chatId, userId);

                case FileType.VoiceMessage when !settings.VoiceMessageEnabled:
                    throw new ChatVoiceMessagesDisabledByUserSettingsException(chatId, userId);

                case FileType.VideoMessage when !settings.VideoMessageEnabled:
                    throw new ChatVideoMessagesDisabledByUserSettingsException(chatId, userId);
            }
        }
    }
}