namespace AIChatWebServer.Repositories.Models
{
    public sealed class VerificationCodeRecord(
        Guid id,
        Guid userId,
        string type,
        string codeHash,
        int attempts,
        DateTime expiresAt,
        DateTime createdAt)
    {
        public Guid Id { get; } = id;
        public Guid UserId { get; } = userId;
        public string Type { get; } = type;
        public string CodeHash { get; } = codeHash;
        public int Attempts { get; } = attempts;
        public DateTime ExpiresAt { get; } = expiresAt;
        public DateTime CreatedAt { get; } = createdAt;
    }
}
