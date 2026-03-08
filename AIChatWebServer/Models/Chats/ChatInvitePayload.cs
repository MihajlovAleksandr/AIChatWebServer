namespace AIChatWebServer.Models.Chats
{
    public record ChatInvitePayload
    (
        Guid ChatId,
        ChatUserRole RoleOnJoin = ChatUserRole.Member,
        string ChatName = "New Chat"
    );
}
