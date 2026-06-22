namespace AIChatWebServer.Models.User
{
    public sealed class SystemUsersOptions
    {
        public Guid DeletedUserId { get; init; }
        public Guid AIId { get; init; }
    }
}