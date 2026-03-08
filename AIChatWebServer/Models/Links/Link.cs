using System.Text.Json;

namespace AIChatWebServer.Models.Links
{
    public sealed class Link
    {
        public Guid Id { get; }
        public string TokenHash { get; }
        public LinkType Type { get; }
        public string Payload { get; }
        public Guid CreatedBy { get; }
        public DateTime? ExpiresAt { get; }
        public int MaxUses { get; }
        public int CurrentUses { get; private set; }
        public bool Revoked { get; private set; }
        public DateTime CreatedAt { get; }

        public Link(
            Guid id,
            string tokenHash,
            LinkType type,
            string payload,
            Guid createdBy,
            DateTime? expiresAt,
            int maxUses,
            int currentUses,
            bool revoked,
            DateTime createdAt)
        {
            if (string.IsNullOrWhiteSpace(tokenHash))
                throw new ArgumentException("Token hash cannot be empty.", nameof(tokenHash));

            if (maxUses <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxUses));

            Id = id;
            TokenHash = tokenHash;
            Type = type;
            Payload = payload;
            CreatedBy = createdBy;
            ExpiresAt = expiresAt;
            MaxUses = maxUses;
            CurrentUses = currentUses;
            Revoked = revoked;
            CreatedAt = createdAt;
        }

        public bool IsExpired(DateTime utcNow) =>
            ExpiresAt.HasValue && ExpiresAt.Value <= utcNow;

        public bool HasAvailableUses() =>
            CurrentUses < MaxUses;

        public bool IsActive(DateTime utcNow) =>
            !Revoked &&
            !IsExpired(utcNow) &&
            HasAvailableUses();

        public void MarkRevoked()
        {
            if (Revoked)
                return;

            Revoked = true;
        }

        public void IncrementUsage()
        {
            if (!HasAvailableUses())
                throw new InvalidOperationException("Max usage limit reached.");

            CurrentUses++;
        }

        public T GetPayload<T>()
        {
            return JsonSerializer.Deserialize<T>(Payload)
                   ?? throw new InvalidOperationException("Invalid payload format.");
        }
    }
}