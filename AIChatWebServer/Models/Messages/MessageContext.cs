namespace AIChatWebServer.Models.Messages
{
    public record MessageContext
    (
        Message Message,
        Guid UserId,
        bool CanSeeOtherUsersStatuses
    );
}
