namespace AIChatWebServer.Models.User
{
    public sealed class UserBan(Guid id, Guid userId, string reason, BanReason reasonCategory, DateTime bannedAt, DateTime bannedUntil)
    {
        public Guid Id { get; } = id;
        public Guid UserId { get; } = userId;
        public string Reason { get; } = reason;
        public BanReason ReasonCategory { get; } = reasonCategory;
        public DateTime BannedAt { get; } = bannedAt;
        public DateTime BannedUntil { get; } = bannedUntil;

        public bool IsActual()
        {
            return BannedUntil >= DateTime.UtcNow;
        }
    }
}
