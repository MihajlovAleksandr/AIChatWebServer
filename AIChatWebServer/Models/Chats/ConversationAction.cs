using AIChatWebServer.Models.Chats.ValidateSettings;

namespace AIChatWebServer.Models.Chats
{
    public abstract record ConversationAction;
    public abstract record ChatAction : ConversationAction;
    public record UpdateNameAction(Guid UserId, string Name) : ChatAction;
    public record EndChatAction(Guid UserId) : ChatAction;
    public record AddUserAction(Guid UserId, Guid AddedUserId, ChatSearchType SearchType, string ChatName = "New Chat") : ChatAction;
    public record RemoveUserAction(Guid UserId, Guid RemovedUserId) : ChatAction;
    public record CallAction(Guid UserId, bool IsVideo) : ConversationAction;
    public record ChangeChatSettingsAction(Guid UserId, Guid TargetUserId, ChatSettings NewSettings) : ChatAction;
    public record ChangeUserSettingsAction(Guid UserId, Guid TargetUserId, UserSettings NewSettings) : ChatAction;
}
