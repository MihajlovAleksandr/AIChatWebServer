using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Chats.ValidateSettings;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.Chat;
using AIChatWebServer.Models.Files;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.ChatPermissions
{
    public class SendMessageChatRule : ConversationActionRuleBase<SendMessageAction>
    {
        public override void Validate(Chat chat, SendMessageAction action)
        {
            ArgumentNullException.ThrowIfNull(chat);

            ArgumentNullException.ThrowIfNull(action);

            ValidateMessageSettings(chat.Id, chat.Settings.Messages, action);
        }

        private static void ValidateMessageSettings(
            Guid chatId,
            MessageSettings settings,
            SendMessageAction action)
        {
            if (action.Attachments == null || action.Attachments.Count == 0)
                return;

            foreach (var attachmentType in action.Attachments)
            {
                ValidateAttachment(chatId, settings, attachmentType);
            }
        }

        private static void ValidateAttachment(
            Guid chatId,
            MessageSettings settings,
            UploadSessionFile file)
        {
            switch (file.ExpectedFileType)
            {
                case FileType.MessageFile when !settings.MessageFilesEnabled:
                    throw new ChatMessageFilesDisabledByChatSettingsException(chatId);

                case FileType.MessageImage when !settings.MessageImagesEnabled:
                    throw new ChatMessageImagesDisabledByChatSettingsException(chatId);

                case FileType.VoiceMessage when !settings.VoiceMessageEnabled:
                    throw new ChatVoiceMessagesDisabledByChatSettingsException(chatId);

                case FileType.VideoMessage when !settings.VideoMessageEnabled:
                    throw new ChatVideoMessagesDisabledByChatSettingsException(chatId);
            }
        }
    }
}
