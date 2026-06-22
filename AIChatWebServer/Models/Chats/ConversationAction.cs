using AIChatWebServer.Models.Chats.ValidateSettings;
using AIChatWebServer.Models.Files;
using AIChatWebServer.Models.Messages;


namespace AIChatWebServer.Models.Chats
{
    public abstract record ConversationAction;
    public abstract record ChatAction : ConversationAction;
    public abstract record MessageAction : ConversationAction;
    public record UpdateNameAction(Guid UserId, string Name) : ChatAction;
    public record EndChatAction(Guid UserId) : ChatAction;
    public record AddUserAction(Guid UserId, Guid AddedUserId, ChatSearchType SearchType,
        ChatUserRole RoleOnJoin = ChatUserRole.Member, string ChatName = "New Chat") : ChatAction;
    public record RemoveUserAction(Guid UserId, Guid RemovedUserId) : ChatAction;
    public record CallAction(Guid UserId, bool IsVideo) : ConversationAction;
    public record ChangeChatSettingsAction(Guid UserId, Guid TargetUserId, ChatSettings NewSettings) : ChatAction;
    public record ChangeUserSettingsAction(Guid UserId, Guid TargetUserId, UserSettings NewSettings) : ChatAction;
    public record InviteUserToChatAction(Guid UserId, ChatUserRole RoleOnJoin = ChatUserRole.Member, string ChatName = "New Chat") : ConversationAction;
    public record StartSearchChatAction(Guid UserId, string UserPredicate, int Slots) : ChatAction;
    public record PrepareSendMessageAction(Guid UserId, int TextLenght,
        IReadOnlyCollection<FileType> AttachmentTypes, int RepliesCount) : MessageAction;
    public record SendMessageAction(Guid UserId, string Text, IReadOnlyCollection<UploadSessionFile> Attachments,
        IReadOnlyCollection<MessageReply> MessageReplies) : MessageAction;
    public record EditMessageAction(Message Message, Guid UserId, string Text) : MessageAction;
    public record DeleteMessageAction(Message Message, Guid UserId) : MessageAction; 
}
