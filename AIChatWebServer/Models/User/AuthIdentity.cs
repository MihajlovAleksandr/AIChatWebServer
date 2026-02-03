namespace AIChatWebServer.Models.User
{
    public sealed class AuthIdentity {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public AuthProvider Provider { get; private set; } = null!;
        public string Identifier { get; private set; } = null!;
        public string? Secret { get; private set; }
        public DateTime CreatedAt { get; private set; }
        private AuthIdentity() { }
        public AuthIdentity(Guid id, Guid userId, AuthProvider provider, string identifier, string? secret)
        { 
            Id = id; 
            UserId = userId;
            Provider = provider; 
            Identifier = identifier;
            Secret = secret;
            CreatedAt = DateTime.UtcNow; }
        public AuthIdentity(AuthProvider provider, string identifier, string? secret)
        {
            Provider = provider;
            Identifier = identifier;
            Secret = secret;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateSecret(string newSecret) { Secret = newSecret; } }
}
