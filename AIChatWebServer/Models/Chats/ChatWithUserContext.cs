namespace AIChatWebServer.Models.Chats
{
    public record ChatWithUserContext
    (
        Chat Chat,
        Guid UserId
    );
}
