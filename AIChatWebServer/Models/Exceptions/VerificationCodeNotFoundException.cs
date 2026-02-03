namespace AIChatWebServer.Models.Exceptions
{
    public sealed class VerificationCodeNotFoundException : Exception
    {
        public Guid UserId { get; }
        public string Type { get; }

        public VerificationCodeNotFoundException(Guid userId, string type)
            : base("Verification code not found")
        {
            UserId = userId;
            Type = type;
        }
    }
}
