namespace AIChatWebServer.Utils.Errors
{
    public class ChatErrors : ErrorCode
    {
        private ChatErrors(string code) : base(code)
        {
        }

        public static readonly IErrorCode ChatNotFound =
            new ChatErrors("CHAT_NOT_FOUND");

        public static readonly IErrorCode ChatTypeNotSupported =
            new ChatErrors("CHAT_TYPE_NOT_SUPPORTED");

        public static readonly IErrorCode UserAlreadyInChat =
            new ChatErrors("USER_ALREADY_IN_CHAT");

        public static readonly IErrorCode UserNotInChat =
            new ChatErrors("USER_NOT_IN_CHAT");

        public static readonly IErrorCode UserDoesNotBelongToChat =
            new ChatErrors("USER_DOES_NOT_BELONG_TO_CHAT");

        public static readonly IErrorCode ChatAlreadyEnded =
            new ChatErrors("CHAT_ALREADY_ENDED");

        public static readonly IErrorCode ChatCannotBeEnded =
            new ChatErrors("CHAT_CANNOT_BE_ENDED");

        public static readonly IErrorCode ChatMembersModificationForbidden =
            new ChatErrors("CHAT_MEMBERS_MODIFICATION_FORBIDDEN");

        public static readonly IErrorCode ChatCallsForbidden =
            new ChatErrors("CHAT_CALLS_FORBIDDEN");

        public static readonly IErrorCode ChatSettingsModificationForbidden =
            new ChatErrors("CHAT_SETTINGS_MODIFICATION_FORBIDDEN");

        public static readonly IErrorCode ChatUserAdditionDisabledByChatSettings =
            new ChatErrors("CHAT_USER_ADDITION_DISABLED_BY_CHAT_SETTINGS");

        public static readonly IErrorCode ChatCallsDisabledByChatSettings =
            new ChatErrors("CHAT_CALLS_DISABLED_BY_CHAT_SETTINGS");

        public static readonly IErrorCode ChatUserAdditionDisabledByUserSettings =
            new ChatErrors("CHAT_USER_ADDITION_DISABLED_BY_USER_SETTINGS");

        public static readonly IErrorCode ChatCallsDisabledByUserSettings =
            new ChatErrors("CHAT_CALLS_DISABLED_BY_USER_SETTINGS");

        public static readonly IErrorCode ChatUserRemovalDisabledByUserSettings =
            new ChatErrors("CHAT_USER_REMOVAL_DISABLED_BY_USER_SETTINGS");

        public static readonly IErrorCode ChatUserRemovalForbiddenDueToRoleHierarchy =
            new ChatErrors("CHAT_USER_REMOVAL_FORBIDDEN_DUE_TO_ROLE_HIERARCHY");

        public static readonly IErrorCode ChatUserSettingsChangeDisabledByUserSettings =
            new ChatErrors("CHAT_USER_SETTINGS_CHANGE_DISABLED_BY_USER_SETTINGS");

        public static readonly IErrorCode ChatUserSettingsChangeForbiddenDueToRoleHierarchy =
            new ChatErrors("CHAT_USER_SETTINGS_CHANGE_FORBIDDEN_DUE_TO_ROLE_HIERARCHY");

        public static readonly IErrorCode ChatUserRoleAssignmentForbidden =
            new ChatErrors("CHAT_USER_ROLE_ASSIGNMENT_FORBIDDEN");

        public static readonly IErrorCode ChatUserPermissionEscalationForbidden =
            new ChatErrors("CHAT_USER_PERMISSION_ESCALATION_FORBIDDEN");

        public static readonly IErrorCode ChatSettingsChangeDisabledByUserSettings =
            new ChatErrors("CHAT_SETTINGS_CHANGE_DISABLED_BY_USER_SETTINGS");

        public static readonly IErrorCode CallsDisabledByUserSettings =
            new ChatErrors("CALLS_DISABLED_BY_USER_SETTINGS");

        public static readonly IErrorCode ChatEndForbiddenForNonOwner =
            new ChatErrors("CHAT_END_FORBIDDEN_FOR_NON_OWNER");

        public static readonly IErrorCode ChatUserInvitationForbiddenDueToRoleHierarchy =
            new ChatErrors("CHAT_USER_INVITATION_FORBIDDEN_DUE_TO_ROLE_HIERARCHY");
    }
}