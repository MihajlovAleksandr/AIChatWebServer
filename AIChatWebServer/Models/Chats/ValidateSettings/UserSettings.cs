namespace AIChatWebServer.Models.Chats.ValidateSettings
{
    public sealed class UserSettings
    {
        public ChatUserRole Role { get; }

        public bool CanRemoveUsers { get; }
        public bool CanAddUsersBySearch {  get; }
        public bool CanAddUserByLink { get; }
        public bool CanChangeUserSettings { get; }
        public bool CanChangeChatSettings { get; }

        public bool CanStartCalls { get; }

        public UserSettings(
            ChatUserRole role,
            bool canAddUsersBySearch,
            bool canAddUserByLink,
            bool canRemoveUsers,
            bool canChangeUserSettings,
            bool canChangeChatSettings,
            bool canStartCalls)
        {
            Role = role;
            CanAddUsersBySearch = canAddUsersBySearch;
            CanAddUserByLink = canAddUserByLink;
            CanRemoveUsers = canRemoveUsers;
            CanChangeUserSettings = canChangeUserSettings;
            CanChangeChatSettings = canChangeChatSettings;
            CanStartCalls = canStartCalls;
        }

        public bool IsHigherThan(UserSettings other)
        {
            return Role > other.Role;
        }

        public bool IsOwner() => Role == ChatUserRole.Owner;

        public bool IsAdmin() => Role == ChatUserRole.Admin;

        public bool IsMember() => Role == ChatUserRole.Member;

        public static UserSettings CreateOwner()
        {
            return new UserSettings(
                role: ChatUserRole.Owner,
                canAddUsersBySearch: true,
                canAddUserByLink: true,
                canRemoveUsers: true,
                canChangeUserSettings: true,
                canChangeChatSettings: true,
                canStartCalls: true);
        }

        public static UserSettings CreateAdmin()
        {
            return new UserSettings(
                role: ChatUserRole.Admin,
                canAddUsersBySearch: true,
                canAddUserByLink: true,
                canRemoveUsers: true,
                canChangeUserSettings: true,
                canChangeChatSettings: true,
                canStartCalls: true);
        }

        public static UserSettings CreateDefaultMember()
        {
            return new UserSettings(
                ChatUserRole.Member,
                canAddUsersBySearch: false,
                canAddUserByLink: false,
                canRemoveUsers: false,
                canChangeUserSettings: false,
                canChangeChatSettings: false,
                canStartCalls: true);
        }
    }
}