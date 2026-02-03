namespace AIChatWebServer.Models.Exceptions
{
    public sealed class VerificationCodeExpiredException : Exception
    {
        public DateTime ExpiredAt { get; }

        public VerificationCodeExpiredException(DateTime expiredAt)
            : base("Verification code expired")
        {
            ExpiredAt = expiredAt;
        }
    }
}
