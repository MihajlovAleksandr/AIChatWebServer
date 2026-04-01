namespace AIChatWebServer.Models.Chats.ValidateSettings
{
    public sealed class UserSettings(
        ChatUserRole role,
        bool canAddUsersBySearch,
        bool canAddUserByLink,
        bool canRemoveUsers,
        bool canChangeUserSettings,
        bool canChangeChatSettings,
        bool canStartCalls,
        UserMessageSettings messages)
    {
        public ChatUserRole Role { get; } = role;

        public bool CanRemoveUsers { get; } = canRemoveUsers;
        public bool CanAddUsersBySearch { get; } = canAddUsersBySearch;
        public bool CanAddUserByLink { get; } = canAddUserByLink;
        public bool CanChangeUserSettings { get; } = canChangeUserSettings;
        public bool CanChangeChatSettings { get; } = canChangeChatSettings;
        public bool CanStartCalls { get; } = canStartCalls;

        public UserMessageSettings Messages { get; } = messages ?? throw new ArgumentNullException(nameof(messages));

        public bool IsHigherThan(UserSettings other) => Role > other.Role;
        public bool IsOwner() => Role == ChatUserRole.Owner;
        public bool IsAdmin() => Role == ChatUserRole.Admin;
        public bool IsMember() => Role == ChatUserRole.Member;

        public static UserSettings CreateOwner()
        {
            return new UserSettings(
                ChatUserRole.Owner,
                true, true, true, true, true, true,
                UserMessageSettings.CreateFullAccess());
        }

        public static UserSettings CreateAdmin()
        {
            return new UserSettings(
                ChatUserRole.Admin,
                true, true, true, true, true, true,
                UserMessageSettings.CreateFullAccess());
        }

        public static UserSettings CreateDefaultMember()
        {
            return new UserSettings(
                ChatUserRole.Member,
                false, false, false, false, false, true,
                UserMessageSettings.CreateDefault());
        }

        public static UserSettings Create(ChatUserRole role)
        {
            return role switch
            {
                ChatUserRole.Member => CreateDefaultMember(),
                ChatUserRole.Admin => CreateAdmin(),
                ChatUserRole.Owner => CreateOwner(),
                _ => throw new NotSupportedException()
            };
        }
    }
}