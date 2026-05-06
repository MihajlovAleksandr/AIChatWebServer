namespace AIChatWebServer.Models.User
{
    public sealed class UserMeta(
        Guid id,
        bool deletedStatus)
    {
        public Guid Id { get; } = id;
        public bool DeletedStatus { get; } = deletedStatus;
    }
}