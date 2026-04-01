using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Chats.ValidateSettings;
using AIChatWebServer.Models.Exceptions.Implementations.Chat;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.SettingsExceptions.User;

namespace AIChatWebServer.Services.Implementations.Chats.ChatPolicyValidator.UserPermissions
{
    public class ChangeUserSettingsUserRule
        : ConversationActionRuleBase<ChangeUserSettingsAction>
    {
        public override void Validate(Chat chat, ChangeUserSettingsAction action)
        {
            if (!chat.UsersWithData.TryGetValue(action.UserId, out ChatUserData? actingUser))
            {
                throw new UserDoesNotBelongToChatException(chat.Id, action.UserId);
            }

            if (!chat.UsersWithData.TryGetValue(action.TargetUserId, out ChatUserData? targetUser))
            {
                throw new UserNotInChatException(chat.Id, action.TargetUserId);
            }

            if (!actingUser.UserSettings.CanChangeUserSettings)
            {
                throw new ChatUserSettingsChangeDisabledByUserSettingsException(
                    chat.Id,
                    action.UserId);
            }

            if (!actingUser.UserSettings.IsHigherThan(targetUser.UserSettings)
                && action.UserId != action.TargetUserId)
            {
                throw new ChatUserSettingsChangeForbiddenDueToRoleHierarchyException(
                    chat.Id,
                    action.UserId,
                    actingUser.UserSettings.Role,
                    action.TargetUserId,
                    targetUser.UserSettings.Role);
            }

            if (action.NewSettings.Role > actingUser.UserSettings.Role)
            {
                throw new ChatUserRoleAssignmentForbiddenException(
                    chat.Id,
                    action.UserId,
                    actingUser.UserSettings.Role,
                    action.NewSettings.Role);
            }

            ValidatePermissionEscalation(
                chat.Id,
                action.UserId,
                actingUser.UserSettings,
                action.TargetUserId,
                targetUser.UserSettings,
                action.NewSettings);

            ValidateMessageSettings(
                chat.Id,
                action.UserId,
                actingUser.UserSettings.Messages,
                action.TargetUserId,
                targetUser.UserSettings.Messages,
                action.NewSettings.Messages);
        }

        private static void ValidatePermissionEscalation(
            Guid chatId,
            Guid actingUserId,
            UserSettings actingSettings,
            Guid targetUserId,
            UserSettings targetCurrentSettings,
            UserSettings newSettings)
        {
            ValidateSinglePermission(
                chatId,
                actingUserId,
                targetUserId,
                nameof(UserSettings.CanAddUsersBySearch),
                actingSettings.CanAddUsersBySearch,
                targetCurrentSettings.CanAddUsersBySearch,
                newSettings.CanAddUsersBySearch);

            ValidateSinglePermission(
                chatId,
                actingUserId,
                targetUserId,
                nameof(UserSettings.CanAddUserByLink),
                actingSettings.CanAddUserByLink,
                targetCurrentSettings.CanAddUserByLink,
                newSettings.CanAddUserByLink);

            ValidateSinglePermission(
                chatId,
                actingUserId,
                targetUserId,
                nameof(UserSettings.CanRemoveUsers),
                actingSettings.CanRemoveUsers,
                targetCurrentSettings.CanRemoveUsers,
                newSettings.CanRemoveUsers);

            ValidateSinglePermission(
                chatId,
                actingUserId,
                targetUserId,
                nameof(UserSettings.CanChangeUserSettings),
                actingSettings.CanChangeUserSettings,
                targetCurrentSettings.CanChangeUserSettings,
                newSettings.CanChangeUserSettings);

            ValidateSinglePermission(
                chatId,
                actingUserId,
                targetUserId,
                nameof(UserSettings.CanChangeChatSettings),
                actingSettings.CanChangeChatSettings,
                targetCurrentSettings.CanChangeChatSettings,
                newSettings.CanChangeChatSettings);

            ValidateSinglePermission(
                chatId,
                actingUserId,
                targetUserId,
                nameof(UserSettings.CanStartCalls),
                actingSettings.CanStartCalls,
                targetCurrentSettings.CanStartCalls,
                newSettings.CanStartCalls);
        }

        private static void ValidateMessageSettings(
            Guid chatId,
            Guid actingUserId,
            UserMessageSettings acting,
            Guid targetUserId,
            UserMessageSettings current,
            UserMessageSettings next)
        {
            ValidateSinglePermission(chatId, actingUserId, targetUserId,
                nameof(UserMessageSettings.MessagesEnabled),
                acting.MessagesEnabled,
                current.MessagesEnabled,
                next.MessagesEnabled);

            ValidateSinglePermission(chatId, actingUserId, targetUserId,
                nameof(UserMessageSettings.MessageFilesEnabled),
                acting.MessageFilesEnabled,
                current.MessageFilesEnabled,
                next.MessageFilesEnabled);

            ValidateSinglePermission(chatId, actingUserId, targetUserId,
                nameof(UserMessageSettings.MessageImagesEnabled),
                acting.MessageImagesEnabled,
                current.MessageImagesEnabled,
                next.MessageImagesEnabled);

            ValidateSinglePermission(chatId, actingUserId, targetUserId,
                nameof(UserMessageSettings.VoiceMessageEnabled),
                acting.VoiceMessageEnabled,
                current.VoiceMessageEnabled,
                next.VoiceMessageEnabled);

            ValidateSinglePermission(chatId, actingUserId, targetUserId,
                nameof(UserMessageSettings.VideoMessageEnabled),
                acting.VideoMessageEnabled,
                current.VideoMessageEnabled,
                next.VideoMessageEnabled);

            ValidateSinglePermission(chatId, actingUserId, targetUserId,
                nameof(UserMessageSettings.EditMessagesEnabled),
                acting.EditMessagesEnabled,
                current.EditMessagesEnabled,
                next.EditMessagesEnabled);

            ValidateSinglePermission(chatId, actingUserId, targetUserId,
                nameof(UserMessageSettings.DeleteOwnMessagesEnabled),
                acting.DeleteOwnMessagesEnabled,
                current.DeleteOwnMessagesEnabled,
                next.DeleteOwnMessagesEnabled);

            ValidateSinglePermission(chatId, actingUserId, targetUserId,
                nameof(UserMessageSettings.DeleteOtherMessagesEnabled),
                acting.DeleteOtherMessagesEnabled,
                current.DeleteOtherMessagesEnabled,
                next.DeleteOtherMessagesEnabled);

            if (!next.MessagesEnabled &&
                (next.MessageFilesEnabled ||
                 next.MessageImagesEnabled ||
                 next.VoiceMessageEnabled ||
                 next.VideoMessageEnabled))
            {
                throw new ChatUserInvalidMessageSettingsException(
                    chatId,
                    actingUserId,
                    targetUserId,
                    "Cannot enable message content types when messages are disabled");
            }

            if (next.DeleteOtherMessagesEnabled && !next.DeleteOwnMessagesEnabled)
            {
                throw new ChatUserInvalidMessageSettingsException(
                    chatId,
                    actingUserId,
                    targetUserId,
                    "Cannot delete others messages without deleting own messages");
            }
        }

        private static void ValidateSinglePermission(
            Guid chatId,
            Guid actingUserId,
            Guid targetUserId,
            string permissionName,
            bool actingValue,
            bool targetCurrentValue,
            bool newValue)
        {
            bool isTryingToEnable = !targetCurrentValue && newValue;
            bool actingDoesNotHavePermission = !actingValue;

            if (isTryingToEnable && actingDoesNotHavePermission)
            {
                throw new ChatUserPermissionEscalationException(
                    chatId,
                    actingUserId,
                    targetUserId,
                    permissionName);
            }
        }
    }
}