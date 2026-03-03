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